export interface LoginRequest {
  email: string;
  password: string;
  userType: 'factory' | 'house';
}
