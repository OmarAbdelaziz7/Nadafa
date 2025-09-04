using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Contracts;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(
            IConfiguration configuration,
            ILogger<SendGridEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var apiKey = _configuration["SendGridSettings:ApiKey"];
            _fromEmail = _configuration["SendGridSettings:FromEmail"];
            _fromName = _configuration["SendGridSettings:FromName"];

            _sendGridClient = new SendGridClient(apiKey);
        }

        public async Task<bool> SendPickupRequestConfirmationAsync(string userEmail, PickupRequest request)
        {
            var subject = "Pickup Request Confirmation - NADAFA";
            var htmlContent = GetPickupRequestConfirmationTemplate(request);
            return await SendEmailAsync(userEmail, subject, htmlContent);
        }

        public async Task<bool> SendPickupApprovalAsync(string userEmail, PickupRequest request, decimal paymentAmount)
        {
            var subject = "Pickup Request Approved - NADAFA";
            var htmlContent = GetPickupApprovedTemplate(request, paymentAmount);
            return await SendEmailAsync(userEmail, subject, htmlContent);
        }

        public async Task<bool> SendPickupRejectionAsync(string userEmail, PickupRequest request, string reason)
        {
            var subject = "Pickup Request Update - NADAFA";
            var htmlContent = GetPickupRejectedTemplate(request, reason);
            return await SendEmailAsync(userEmail, subject, htmlContent);
        }

        public async Task<bool> SendPaymentConfirmationAsync(string userEmail, decimal amount, string description)
        {
            var subject = "Payment Confirmation - NADAFA";
            var htmlContent = GetPaymentReceivedTemplate(amount, description);
            return await SendEmailAsync(userEmail, subject, htmlContent);
        }

        public async Task<bool> SendItemSoldNotificationAsync(string userEmail, MarketplaceItem item, Purchase purchase)
        {
            var subject = "Item Sold - NADAFA Marketplace";
            var htmlContent = GetItemSoldTemplate(item, purchase);
            return await SendEmailAsync(userEmail, subject, htmlContent);
        }

        public async Task<bool> SendAdminNewRequestAsync(string adminEmail, PickupRequest request)
        {
            var subject = "New Pickup Request - NADAFA Admin";
            var htmlContent = GetNewRequestForAdminTemplate(request);
            return await SendEmailAsync(adminEmail, subject, htmlContent);
        }

        public async Task<bool> SendPaymentProcessingConfirmationAsync(string adminEmail, Payment payment)
        {
            var subject = "Payment Processing - NADAFA Admin";
            var htmlContent = GetPaymentProcessingTemplate(payment);
            return await SendEmailAsync(adminEmail, subject, htmlContent);
        }

        public async Task<bool> SendPurchaseConfirmationAsync(string factoryEmail, Purchase purchase)
        {
            var subject = "Purchase Confirmation - NADAFA";
            var htmlContent = GetPurchaseConfirmationTemplate(purchase);
            return await SendEmailAsync(factoryEmail, subject, htmlContent);
        }

        public async Task<bool> SendPaymentReceiptAsync(string factoryEmail, Payment payment)
        {
            var subject = "Payment Receipt - NADAFA";
            var htmlContent = GetPaymentReceiptTemplate(payment);
            return await SendEmailAsync(factoryEmail, subject, htmlContent);
        }

        public async Task<bool> SendCustomEmailAsync(string toEmail, string subject, string htmlContent)
        {
            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

        public async Task<bool> SendBulkEmailAsync(string[] toEmails, string subject, string htmlContent)
        {
            try
            {
                var message = new SendGridMessage();
                message.SetFrom(_fromEmail, _fromName);
                message.SetSubject(subject);
                message.AddContent(MimeType.Html, htmlContent);

                foreach (var email in toEmails)
                {
                    message.AddTo(email);
                }

                var response = await _sendGridClient.SendEmailAsync(message);
                _logger.LogInformation($"Bulk email sent to {toEmails.Length} recipients. Status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk email");
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var message = new SendGridMessage();
                message.SetFrom(_fromEmail, _fromName);
                message.AddTo(toEmail);
                message.SetSubject(subject);
                message.AddContent(MimeType.Html, htmlContent);

                var response = await _sendGridClient.SendEmailAsync(message);
                _logger.LogInformation($"Email sent to {toEmail}. Status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail}");
                return false;
            }
        }

        // Email Template Methods
        private string GetPickupRequestConfirmationTemplate(PickupRequest request)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Pickup Request Confirmation</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4CAF50, #45a049); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #4CAF50; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Pickup Request Confirmation</p>
                        </div>
                        <div class='content'>
                            <h2>Your pickup request has been submitted successfully!</h2>
                            <p>Thank you for choosing NADAFA for your recycling needs. We have received your pickup request and will review it shortly.</p>
                            
                            <div class='details'>
                                <h3>Request Details:</h3>
                                <p><strong>Request ID:</strong> {request.Id}</p>
                                <p><strong>Material Type:</strong> {request.MaterialType}</p>
                                <p><strong>Quantity:</strong> {request.Quantity} {request.Unit}</p>
                                <p><strong>Proposed Price:</strong> ${request.ProposedPricePerUnit:F2} per unit</p>
                                <p><strong>Description:</strong> {request.Description}</p>
                                <p><strong>Request Date:</strong> {request.RequestDate:MMM dd, yyyy}</p>
                            </div>
                            
                            <p>We will notify you once your request has been reviewed. You can track the status of your request through your NADAFA dashboard.</p>
                            
                            <p>If you have any questions, please don't hesitate to contact our support team.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPickupApprovedTemplate(PickupRequest request, decimal paymentAmount)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Pickup Request Approved</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4CAF50, #45a049); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .success {{ background: #d4edda; color: #155724; padding: 15px; border-radius: 5px; border: 1px solid #c3e6cb; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #4CAF50; }}
                        .payment {{ background: #e8f5e8; padding: 15px; margin: 15px 0; border-radius: 5px; border: 1px solid #4CAF50; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Pickup Request Approved</p>
                        </div>
                        <div class='content'>
                            <div class='success'>
                                <h2>🎉 Congratulations! Your pickup request has been approved!</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Request Details:</h3>
                                <p><strong>Request ID:</strong> {request.Id}</p>
                                <p><strong>Material Type:</strong> {request.MaterialType}</p>
                                <p><strong>Quantity:</strong> {request.Quantity} {request.Unit}</p>
                                <p><strong>Approved Date:</strong> {request.ApprovedDate:MMM dd, yyyy}</p>
                                {(!string.IsNullOrEmpty(request.AdminNotes) ? $"<p><strong>Admin Notes:</strong> {request.AdminNotes}</p>" : "")}
                            </div>
                            
                            <div class='payment'>
                                <h3>Payment Information:</h3>
                                <p><strong>Payment Amount:</strong> ${paymentAmount:F2}</p>
                                <p>Payment will be processed within 24-48 hours after pickup completion.</p>
                            </div>
                            
                            <p>Our team will contact you shortly to schedule the pickup. Please ensure your materials are ready and accessible.</p>
                            
                            <p>Thank you for contributing to a greener environment with NADAFA!</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPickupRejectedTemplate(PickupRequest request, string reason)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Pickup Request Update</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #ff9800, #f57c00); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .notice {{ background: #fff3cd; color: #856404; padding: 15px; border-radius: 5px; border: 1px solid #ffeaa7; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #ff9800; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Pickup Request Update</p>
                        </div>
                        <div class='content'>
                            <div class='notice'>
                                <h2>📋 Pickup Request Status Update</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Request Details:</h3>
                                <p><strong>Request ID:</strong> {request.Id}</p>
                                <p><strong>Material Type:</strong> {request.MaterialType}</p>
                                <p><strong>Quantity:</strong> {request.Quantity} {request.Unit}</p>
                                <p><strong>Status:</strong> Rejected</p>
                                <p><strong>Reason:</strong> {reason}</p>
                            </div>
                            
                            <p>We regret to inform you that your pickup request could not be approved at this time. Please review the reason provided above and consider submitting a new request with the necessary adjustments.</p>
                            
                            <p>If you have any questions or need assistance, please contact our support team.</p>
                            
                            <p>Thank you for your understanding and continued support of NADAFA.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPaymentReceivedTemplate(decimal amount, string description)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Payment Confirmation</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .success {{ background: #d4edda; color: #155724; padding: 15px; border-radius: 5px; border: 1px solid #c3e6cb; }}
                        .payment {{ background: #e3f2fd; padding: 15px; margin: 15px 0; border-radius: 5px; border: 1px solid #2196F3; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Payment Confirmation</p>
                        </div>
                        <div class='content'>
                            <div class='success'>
                                <h2>💰 Payment Received Successfully!</h2>
                            </div>
                            
                            <div class='payment'>
                                <h3>Payment Details:</h3>
                                <p><strong>Amount:</strong> ${amount:F2}</p>
                                <p><strong>Description:</strong> {description}</p>
                                <p><strong>Date:</strong> {DateTime.UtcNow:MMM dd, yyyy 'at' HH:mm UTC}</p>
                            </div>
                            
                            <p>Your payment has been processed successfully. Thank you for your contribution to sustainable recycling!</p>
                            
                            <p>You can view your transaction history in your NADAFA dashboard.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetItemSoldTemplate(MarketplaceItem item, Purchase purchase)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Item Sold - NADAFA Marketplace</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #9C27B0, #7B1FA2); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .success {{ background: #d4edda; color: #155724; padding: 15px; border-radius: 5px; border: 1px solid #c3e6cb; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #9C27B0; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Item Sold - Marketplace</p>
                        </div>
                        <div class='content'>
                            <div class='success'>
                                <h2>🎉 Congratulations! Your item has been sold!</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Sale Details:</h3>
                                <p><strong>Item Title:</strong> {item.Title}</p>
                                <p><strong>Sale Price:</strong> ${purchase.TotalAmount:F2}</p>
                                <p><strong>Sale Date:</strong> {purchase.PurchaseDate:MMM dd, yyyy}</p>
                                <p><strong>Transaction ID:</strong> {purchase.Id}</p>
                            </div>
                            
                            <p>Your payment will be processed within 24-48 hours. You can track the status in your NADAFA dashboard.</p>
                            
                            <p>Thank you for using NADAFA Marketplace!</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetNewRequestForAdminTemplate(PickupRequest request)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>New Pickup Request - Admin</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #FF5722, #E64A19); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .alert {{ background: #fff3cd; color: #856404; padding: 15px; border-radius: 5px; border: 1px solid #ffeaa7; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #FF5722; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>New Pickup Request - Admin Notification</p>
                        </div>
                        <div class='content'>
                            <div class='alert'>
                                <h2>⚠️ New Pickup Request Requires Review</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Request Details:</h3>
                                <p><strong>Request ID:</strong> {request.Id}</p>
                                <p><strong>User ID:</strong> {request.UserId}</p>
                                <p><strong>Material Type:</strong> {request.MaterialType}</p>
                                <p><strong>Quantity:</strong> {request.Quantity} {request.Unit}</p>
                                <p><strong>Proposed Price:</strong> ${request.ProposedPricePerUnit:F2} per unit</p>
                                <p><strong>Description:</strong> {request.Description}</p>
                                <p><strong>Request Date:</strong> {request.RequestDate:MMM dd, yyyy 'at' HH:mm}</p>
                            </div>
                            
                            <p>Please review this pickup request and take appropriate action through the admin dashboard.</p>
                            
                            <p><strong>Action Required:</strong> Approve or reject this request within 24 hours.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPaymentProcessingTemplate(Payment payment)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Payment Processing - Admin</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #607D8B, #455A64); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .info {{ background: #e3f2fd; color: #1565C0; padding: 15px; border-radius: 5px; border: 1px solid #2196F3; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #607D8B; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Payment Processing Notification</p>
                        </div>
                        <div class='content'>
                            <div class='info'>
                                <h2>💳 Payment Processing Update</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Payment Details:</h3>
                                <p><strong>Payment ID:</strong> {payment.Id}</p>
                                <p><strong>Amount:</strong> ${payment.Amount:F2}</p>
                                <p><strong>Status:</strong> {payment.Status}</p>
                                <p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>
                                <p><strong>Date:</strong> {payment.PaymentDate:MMM dd, yyyy 'at' HH:mm}</p>
                            </div>
                            
                            <p>This payment has been processed successfully. The transaction has been recorded in the system.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPurchaseConfirmationTemplate(Purchase purchase)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Purchase Confirmation - NADAFA</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #4CAF50, #45a049); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .success {{ background: #d4edda; color: #155724; padding: 15px; border-radius: 5px; border: 1px solid #c3e6cb; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #4CAF50; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Purchase Confirmation</p>
                        </div>
                        <div class='content'>
                            <div class='success'>
                                <h2>✅ Purchase Confirmed!</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Purchase Details:</h3>
                                <p><strong>Purchase ID:</strong> {purchase.Id}</p>
                                <p><strong>Total Amount:</strong> ${purchase.TotalAmount:F2}</p>
                                <p><strong>Purchase Date:</strong> {purchase.PurchaseDate:MMM dd, yyyy}</p>
                                <p><strong>Status:</strong> Confirmed</p>
                            </div>
                            
                            <p>Your purchase has been confirmed successfully. You will receive the materials as scheduled.</p>
                            
                            <p>Thank you for choosing NADAFA for your recycling materials!</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }

        private string GetPaymentReceiptTemplate(Payment payment)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>Payment Receipt - NADAFA</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #2196F3, #1976D2); color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 20px; border-radius: 0 0 8px 8px; }}
                        .receipt {{ background: #e8f5e8; padding: 15px; margin: 15px 0; border-radius: 5px; border: 1px solid #4CAF50; }}
                        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #2196F3; }}
                        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>NADAFA Recycling Platform</h1>
                            <p>Payment Receipt</p>
                        </div>
                        <div class='content'>
                            <div class='receipt'>
                                <h2>📄 Payment Receipt</h2>
                            </div>
                            
                            <div class='details'>
                                <h3>Payment Details:</h3>
                                <p><strong>Receipt ID:</strong> {payment.Id}</p>
                                <p><strong>Amount:</strong> ${payment.Amount:F2}</p>
                                <p><strong>Payment Method:</strong> {payment.PaymentMethod}</p>
                                <p><strong>Status:</strong> {payment.Status}</p>
                                <p><strong>Date:</strong> {payment.PaymentDate:MMM dd, yyyy 'at' HH:mm}</p>
                            </div>
                            
                            <p>This serves as your official payment receipt. Please keep this for your records.</p>
                            
                            <p>Thank you for your business with NADAFA!</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 NADAFA Recycling Platform. All rights reserved.</p>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
