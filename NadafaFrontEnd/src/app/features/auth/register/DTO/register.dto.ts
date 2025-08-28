export interface RegisterDto {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
  userType: 'factory' | 'house';
}
