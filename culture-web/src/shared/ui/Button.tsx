import type { ButtonHTMLAttributes, ReactNode } from 'react';
import { clsx } from 'clsx';

type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost';

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  icon?: ReactNode;
  variant?: ButtonVariant;
};

export function Button({ className, icon, children, variant = 'primary', ...props }: ButtonProps) {
  return (
    <button className={clsx('btn', `btn-${variant}`, className)} type="button" {...props}>
      {icon}
      <span>{children}</span>
    </button>
  );
}
