import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatchValidator<T extends { password: string; confirmPassword: string }>(
  passwordKey: keyof T = 'password',
  confirmKey: keyof T = 'confirmPassword',
): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const password = group.get(passwordKey as string)?.value;
    const confirm = group.get(confirmKey as string)?.value;
    return password && confirm && password !== confirm ? { passwordMismatch: true } : null;
  };
}