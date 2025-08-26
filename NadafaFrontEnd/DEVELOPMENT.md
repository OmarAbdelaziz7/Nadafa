# 🚀 Development Guide - Nadafa

## Development Authentication Bypass

For faster development and testing, the application includes special development credentials that bypass the actual authentication flow.

### Quick Access Methods

#### 1. **Development Credentials**
When running in development mode, you'll see a blue info box on the login page with:
- **Email**: `dev@nadafa.local`
- **Password**: `dev123`

#### 2. **Quick Fill Button**
Click the "⚡ Quick Fill" button to automatically populate the login form with development credentials.

#### 3. **Skip Auth Button**
Click the "⏭️ Skip Auth" button to completely bypass the login form and go directly to the dashboard.

### How It Works

The development bypass:
- ✅ **Only works in development mode** (localhost, 127.0.0.1)
- ✅ **Automatically disabled in production builds**
- ✅ **Shows clear visual indicators** when active
- ✅ **Uses special tokens** that can be identified (`dev-bypass-token`)

### Security Notes

⚠️ **Important**: 
- Development credentials are **automatically disabled** in production
- The bypass only works when `location.hostname` is localhost or 127.0.0.1
- Production builds will not include these features

### Development Workflow

1. **Start the dev server**: `npm start`
2. **Navigate to**: `http://localhost:4200`
3. **Choose your method**:
   - Use the quick fill + login
   - Use the skip auth button
   - Manually enter dev credentials

### Files Modified

- `src/app/core/auth/auth.service.ts` - Added dev credential checking
- `src/app/features/auth/login/login.component.ts` - Added dev UI and bypass methods
- `src/environments/environment.ts` - Development configuration

### Removing Development Features

When ready for production:
1. Ensure you're using the production environment
2. The development features will automatically be disabled
3. Consider removing the dev credential code entirely from AuthService if desired

---

Happy coding! 🎉
