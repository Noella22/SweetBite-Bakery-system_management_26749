-- OPTIONAL: Run in phpMyAdmin only after backing up sweetbite_db.
-- Keeps existing products and sales rows. Adds tables/columns for extended SRS.
-- MariaDB 10.4+ (your XAMPP screenshot shows MariaDB 10.4.32).
USE sweetbite_db;

ALTER TABLE products
    ADD COLUMN IF NOT EXISTS category VARCHAR(80) NOT NULL DEFAULT 'Bakery',
    ADD COLUMN IF NOT EXISTS stock_qty INT NOT NULL DEFAULT 0,
    ADD COLUMN IF NOT EXISTS is_available TINYINT(1) NOT NULL DEFAULT 1;

CREATE TABLE IF NOT EXISTS users (
    user_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(80) NOT NULL UNIQUE,
    password_hash VARCHAR(300) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS customers (
    customer_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    customer_name VARCHAR(100) NOT NULL,
    phone VARCHAR(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS orders (
    order_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    order_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(12,2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_sweetbite_customer FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS order_items (
    order_item_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    product_name VARCHAR(150) NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(12,2) NOT NULL,
    line_total DECIMAL(12,2) NOT NULL,
    CONSTRAINT fk_sweetbite_order FOREIGN KEY (order_id)
        REFERENCES orders(order_id),
    CONSTRAINT fk_sweetbite_product FOREIGN KEY (product_id)
        REFERENCES products(product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS payments (
    payment_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    order_id INT NOT NULL,
    payment_method VARCHAR(50) NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    payment_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sweetbite_payment_order FOREIGN KEY (order_id)
        REFERENCES orders(order_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Next: open the application, choose CREATE FIRST ACCOUNT on Login.
-- After the account is created, sign in with that username and password.
-- Existing products will initially have stock_qty = 0; set quantities in Inventory.
