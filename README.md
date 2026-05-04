# MIST-452-Spring26 — BooksSpring26

ASP.NET Core 10 MVC bookstore project for MIST 452 (Spring 2026).

## URL
mist452-invidual-project.azurewebsites.net

## Roles

The app seeds two roles on first startup (see `Data/DbInitializer.cs`):

| Role | What it can do |
|---|---|
| **Admin** | Full access to the `/Admin` area: manage Books, Categories, Orders, and Users (including granting/revoking the Admin role and locking accounts) |
| **Customer** | Browse books, add to cart, checkout via Stripe, view their own orders |

`[Authorize(Roles = "Admin")]` is applied to every Admin controller, so non-admins get an Access Denied page if they try to navigate to `/Admin/...`.

## Default seeded admin account

A default admin user is created automatically the first time the app starts:

| Field | Value |
|---|---|
| Email | `admin@bookssite.local` |
| Password | `Admin#12345` |
| Role | `Admin` |

> **Change this password immediately** after first login (top-right "Hello ..." link → Manage account → Password). The seed only runs if no user with that email exists, so changing the password won't get overwritten.

## Example test accounts

These are not seeded — register them through the `/Identity/Account/Register` page to test the customer flow:

| Purpose | Email | Password | Notes |
|---|---|---|---|
| Customer (regular shopper) | `customer1@test.local` | `Customer#1` | Auto-assigned `Customer` role on registration |
| Customer (second buyer) | `customer2@test.local` | `Customer#2` | Use to verify cart/order isolation between users |
| Promoted admin | `manager@test.local` | `Manager#1` | Register, then log in as the seeded admin and use `/Admin/User` → **Make Admin** |

> Identity password rules require: minimum 6 characters, at least one uppercase, one lowercase, one digit, and one non-alphanumeric character. The examples above already satisfy this.

## Stripe test cards

Use Stripe's test card numbers at checkout (any future expiry, any CVC, any ZIP):

| Card number | Result |
|---|---|
| `4242 4242 4242 4242` | Successful payment |
| `4000 0000 0000 9995` | Declined (insufficient funds) |
| `4000 0025 0000 3155` | Requires 3D Secure authentication |

Configure your Stripe keys in `appsettings.json` (or user-secrets):

```json
"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_..."
}
```

## First-run setup

```bash
dotnet ef database update
dotnet run
```

