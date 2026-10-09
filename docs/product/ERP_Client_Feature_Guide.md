# ERP System: Client Feature Guide

9 October 2026

## Overview

The ERP is one online system that runs your shop counters, stock, purchasing, finance and reports across every branch you own. It is built for busy retail shops, stores, distributors, service businesses and multi-branch groups.

Most of how the system behaves is set from admin screens, not by custom programming. You choose which modules are switched on, who can do what, your taxes, prices, document layouts and reports.

What you get:

- **Fast POS** for busy counters: barcode-first, keyboard shortcuts, touch friendly.
- **Many branches, warehouses and tills** under one account, each with its own stock and prices.
- **Fine-grained access control**: each user sees only the actions and branches their role allows.
- **Full audit trail**: every sale, change and export records who did it and when.
- **Your data stays yours**: each company's data is fully separated from every other customer.
- **Light, dark and branded themes**, plus multiple languages.

One rule runs through the whole system: an original sale is never edited or deleted. Corrections, report adjustments and tax views are kept as separate, traceable records, so your books always reconcile.

## Getting started

You go live by configuring the system, not by waiting for custom code. Your administrator follows these steps once:

1. **Create your company account** (we set this up with you).
2. **Choose your business type**: retail, wholesale, restaurant, service or distribution.
3. **Add branches, warehouses and tills** (POS terminals).
4. **Switch on the modules** you need for your plan.
5. **Define roles** and invite users.
6. **Set up taxes, pricing and approval workflows.**
7. **Configure reports** and saved views.
8. **Customise documents**: invoices, receipts, quotations, delivery notes, statements.
9. **Go live.**

### How your account is organised

| Level | What it is | Example |
| --- | --- | --- |
| Company | Your account and all your business data | ABC Traders |
| Business unit | A division or line of business | Retail, Wholesale |
| Branch | A shop or operating location | Main Street outlet |
| Warehouse | Where stock is held and moved | Branch 03 back store |
| Terminal | A till or mobile POS device | Counter 2 |
| User | A person who logs in | Cashier, manager, owner |
| Role | A job role with permissions and scope | Store Manager |

### Logging in and security

- Each user logs in with their own account. Never share logins: every action is recorded against the user.
- Admins and owners should switch on two-step verification (MFA).
- Repeated wrong passwords lock the account for a period.
- If our support team needs to look at your account, you grant a time-limited session with a stated reason. It is logged and ends automatically.

### Roles and what users can do

Every action is a permission named Module, Resource, Action, for example **Sales.Invoice.Create** or **Inventory.StockTransfer.Approve**. A role bundles permissions and a scope.

| Part | Options |
| --- | --- |
| Action | Create, Read, Update, Delete, Approve, Export, Print |
| Scope | All branches, selected branches, own branch, own records |
| Workflow role | Maker, Checker, Approver |
| Conditions | For example amount up to a limit, or status is Draft |

Example: a cashier can create sales at their own branch but needs a manager's approval for discounts above their limit.

## Available features

The system has 21 modules; your plan decides which are switched on. Admins can turn modules on or off per branch or role without any code change.

| Module | What you can do |
| --- | --- |
| Dashboard | See KPIs, alerts, tasks and pending approvals; compare branches |
| POS | Scan barcodes, search fast, apply discounts, hold carts, split payments, process returns, print receipts |
| Sales | Create quotes, orders and invoices; credit sales, returns and payments |
| Purchasing | Manage suppliers, purchase orders, goods received notes (GRN), supplier invoices and returns |
| Inventory | Products, variants, batches, serial numbers, expiry, transfers, adjustments and stock counts |
| Warehouse | Locations, bins, receiving, picking and transfers |
| Customers | Profiles, pricing groups, credit limits, loyalty and statements |
| Suppliers | Profiles, payment terms, balances and purchase history |
| Finance | Cash and bank, receivables, payables, expenses, journals and reconciliation |
| Tax | Tax profiles, rates, exemptions, periods and auditable tax rules |
| Reports | Operational, financial, inventory, tax and management reports |
| HR-lite | Employees, roles, attendance hooks and sales commissions |
| CRM-lite | Leads, follow-ups and customer notes |
| Expenses | Expense categories, approvals, attachments and branch allocation |
| Assets | Asset register with optional depreciation |
| Documents | Invoices, receipts, quotations, delivery notes and statements from your templates |
| Workflow | Approvals, thresholds, maker-checker and escalations |
| Notifications | Email, SMS and WhatsApp messages plus in-app alerts |
| Integrations | Payment providers, accounting, e-commerce and external APIs |
| Administration | Users, roles, branches, modules, rules, audit, backup and settings |
| Rules & reporting adjustments | Cutoff dates, report exclusions, branch groupings, tax mapping and rounding, without touching original sales |

### What each plan includes

| Plan | Typical scope |
| --- | --- |
| Starter | 1 branch, limited users, POS + Sales + Inventory |
| Business | Multiple branches, advanced reports, Purchasing, approvals |
| Professional | Advanced rules, integrations, automation and analytics |
| Enterprise | Custom limits, dedicated database option, single sign-on (SSO) and SLA |

Pricing is based on number of branches, active users and optional modules. Add-ons are available for advanced reporting, integrations, messaging and storage.

### What you can configure yourself

| Setting | Can be set per |
| --- | --- |
| Modules and features | Plan, company, business unit, branch, role |
| Business rules | Company, branch, till |
| Approval rules | Transaction type, amount, user, branch |
| Tax rules | Company, branch, product or category, effective date |
| Pricing | Customer group, branch, product or category, date range |
| Report views | Report, role, user |
| Theme and language | Company, branch, user |
| Document templates | Document type, branch |
| Notifications | Event, role, user |

Security, data separation between customers, payment security and core audit cannot be switched off. They protect you and are always on.

## How to use each feature

These are the everyday tasks, in the order a shop typically meets them. What you see depends on your role's permissions.

### Run the till (POS)

1. **Open your shift**: log in at the terminal and record the opening cash in the drawer.
2. **Pick the customer**: walk-in, or search for a registered customer.
3. **Add items**: scan the barcode, or search by barcode, SKU, name or alias. Use keyboard shortcuts for speed.
4. **Apply a discount** if needed. Above your limit, the system asks for a manager's approval.
5. **Take payment**: cash, card, bank transfer, or split across several methods.
6. **Print or send the receipt.** Reprinting later needs permission.
7. **Close your shift**: count the drawer and reconcile cash at end of day.

Busy counter tips:

- **Hold a cart** to serve the next customer, then resume it.
- If the till crashes mid-sale, the suspended transaction is recovered when you log back in.
- With the optional offline mode, sales queue during an internet outage and sync once, without duplicates.

### Process a return or exchange

1. Find the original invoice by receipt number or customer.
2. Select the items being returned and the reason.
3. Refund, or add replacement items for an exchange.

The return links to the original sale. The original sale itself is never changed.

### Create a quote, order or invoice

1. Open **Sales** and choose quote, order or invoice.
2. Add the customer, items and quantities. Prices follow the customer's pricing group and branch.
3. For a credit sale, the system checks the customer's credit limit.
4. Save, then print or email it using your document template.
5. Convert a quote to an order, or an order to an invoice, as the deal progresses.

### Manage products and stock

- **Add a product**: simple item, variant (size or colour), bundle or service. Enter SKU, barcodes, supplier code and unit of measure.
- **Track batches, expiry dates or serial numbers** where needed.
- **Set minimum, reorder and maximum levels** per branch to get low-stock alerts.
- **Transfer stock** between branches or warehouses. Transfers can require approval.
- **Run a stock count**: count, review the variances, then approve the adjustment.

Every stock change is kept as a movement record, so any balance can be traced back to the transactions behind it.

### Buy from suppliers

1. Raise a **purchase order** to the supplier.
2. When goods arrive, record a **GRN** (goods received note). Stock goes up.
3. Enter the **supplier invoice** and match it to the PO and GRN.
4. Record the **supplier payment**. Return faulty goods with a supplier return.

### Get approvals

Rules decide what needs sign-off, for example discounts above a limit, large expenses or stock transfers. Pending requests appear on the approver's dashboard. One person prepares (maker), another approves (checker). Unanswered requests escalate.

### Run and share reports

1. Open **Reports** and pick from Sales, Purchases, Inventory, Finance, Tax, Customers, Suppliers or Operations.
2. Filter by branch, date, product, category, user or payment method.
3. **Save the view** to reuse it, for yourself or your role.
4. **Export** to PDF, Excel or CSV if your role allows. Exports are logged.
5. **Schedule** a report to be emailed automatically, for example daily sales every morning.

### Set up a reporting rule (cutoff, exclusion, tax mapping)

Use rules when a report must show figures differently from the raw sales, without altering them. Example: sales after 14:00 at Branch 03 count towards the next business day.

1. Go to **Administration > Rules** and create a rule in **Draft**.
2. Set the **condition** (time after 14:00, Branch 03) and the **action** (reporting date = next business day).
3. Set the effective dates and priority.
4. **Validate** and **simulate** it to preview the effect on past data.
5. **Publish** it.

The original sale time never changes. Reports show both the source total and the adjusted total, and record which rule version changed what. To undo, deactivate the rule or publish a new version.

### Administer users and settings

- **Invite users** and assign roles and branch scope.
- **Switch modules** on or off and adjust settings per branch.
- **Edit document templates**, numbering, taxes, currencies and themes.
- **Review the audit log** to see who changed what, including logins, failed logins, permission changes and exports.

Setting changes are versioned, so a mistake can be rolled back.

## FAQs and support

**Can I delete or edit a sale that was entered wrongly?** No. Use a return, a void (with permission) or an adjustment. The original stays on record so your books and stock always reconcile.

**Can other customers see my data?** No. Each company's data is kept separate and every request is checked against your account. Enterprise plans can have their own dedicated database.

**Can I change how a report shows sales without changing the sales?** Yes. Use a reporting rule (see *Set up a reporting rule*). The report shows both the original and the adjusted totals.

**Who checks that my tax setup is legally correct?** You do, with your accountant or tax adviser. The system applies the rules you set and logs them, but local tax treatment must be reviewed by a professional.

**Why can't a user see a button or a branch?** Their role lacks that permission or scope. An admin can update the role under **Administration > Roles**.

**What happens if the internet drops at the till?** With offline mode on, sales queue on the till and sync once the connection returns, with no duplicates.

**Can I export my data?** Yes, reports export to PDF, Excel and CSV for roles with export permission. Every export is logged.

### Good habits

- Give each person their own login and the smallest role they need.
- Turn on two-step verification for owners and admins.
- Test new rules with **simulate** before publishing.
- Close and reconcile every shift at the end of the day.
- Review the audit log and the approvals queue weekly.

### Getting help

Contact our support team through the in-app help or your account manager. If they need to look inside your account, you approve a time-limited, logged support session. Planned maintenance is announced in the app in advance.
