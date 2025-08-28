export interface LoginDto {
  email: string;
  password: string;
  userType: 'factory' | 'house';
}
