import pandas as pd
import pyodbc

# Configuration
SERVER = 'DESKTOP-PDOVJGU'
DATABASE = 'SalesAnalyticsDB'
CSV_FILE = 'database/cleaned-data/cleaned_data.csv'

# Connection String
conn_str = (
    'DRIVER={ODBC Driver 17 for SQL Server};'  
    f'SERVER={SERVER};'
    f'DATABASE={DATABASE};'
    'Trusted_Connection=yes;'
)

print("📂 Loading data...")
df = pd.read_csv(CSV_FILE)
print(f" Loaded {len(df):,} rows\n")

print("🔌 Connecting to database...")
conn = pyodbc.connect(conn_str)
cursor = conn.cursor()
print(" Successfully connected!\n")

# 1. Insert Regions
print(" Inserting Regions...")
countries = df['Country'].unique()
for country in countries:
    cursor.execute("""
        IF NOT EXISTS (SELECT 1 FROM Regions WHERE Country = ?)
        INSERT INTO Regions (Country) VALUES (?)
    """, str(country), str(country))
conn.commit()
print(f" Inserted {len(countries)} regions\n")

# 2. Insert Products
print(" Inserting Products...")
products = df[['StockCode', 'Description']].drop_duplicates()
for _, row in products.iterrows():
    cursor.execute("""
        IF NOT EXISTS (SELECT 1 FROM Products WHERE StockCode = ?)
        INSERT INTO Products (StockCode, Description) VALUES (?, ?)
    """, str(row['StockCode']), str(row['StockCode']), str(row['Description']))
conn.commit()
print(f" Inserted {len(products):,} products\n")

# 3. Insert Customers
print("👥 Inserting Customers...")
customers = df['CustomerID'].unique()
for customer in customers:
    cursor.execute("""
        IF NOT EXISTS (SELECT 1 FROM Customers WHERE CustomerCode = ?)
        INSERT INTO Customers (CustomerCode) VALUES (?)
    """, str(customer), str(customer))
conn.commit()
print(f" Inserted {len(customers):,} customers\n")

# 4. Insert Sales
print("💰 Inserting Sales (this will take ~30 minutes for 400K rows)...\n")
inserted = 0
errors = 0

for idx, row in df.iterrows():
    try:
        # Get foreign keys
        cursor.execute("SELECT ProductId FROM Products WHERE StockCode = ?", str(row['StockCode']))
        result = cursor.fetchone()
        if not result:
            errors += 1
            continue
        product_id = result[0]
        
        cursor.execute("SELECT CustomerId FROM Customers WHERE CustomerCode = ?", str(row['CustomerID']))
        result = cursor.fetchone()
        if not result:
            errors += 1
            continue
        customer_id = result[0]
        
        cursor.execute("SELECT RegionId FROM Regions WHERE Country = ?", str(row['Country']))
        result = cursor.fetchone()
        if not result:
            errors += 1
            continue
        region_id = result[0]
        
        # Insert sale
        cursor.execute("""
            INSERT INTO Sales (InvoiceNo, ProductId, CustomerId, RegionId, 
                               Quantity, UnitPrice, TotalAmount, InvoiceDate)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """, 
            str(row['InvoiceNo']),
            int(product_id),
            int(customer_id),
            int(region_id),
            int(row['Quantity']),
            float(row['UnitPrice']),
            float(row['TotalAmount']),
            str(row['InvoiceDate'])
        )
        
        inserted += 1
        
        if inserted % 5000 == 0:
            conn.commit()
            percentage = (inserted / len(df)) * 100
            print(f"  Progress: {inserted:,}/{len(df):,} ({percentage:.1f}%)")
            
    except Exception as e:
        errors += 1
        if errors <= 5:
            print(f"⚠️ Error at row {idx}: {e}")
        continue

# Final commit
conn.commit()
print(f"\n{'='*60}")
print(f" Successfully inserted {inserted:,} sales transactions!")
if errors > 0:
    print(f" Skipped {errors:,} rows due to errors")
print(f"{'='*60}")

cursor.close()
conn.close()
print("\n DATABASE IMPORT COMPLETE!")