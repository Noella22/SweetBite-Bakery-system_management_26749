-- Run in phpMyAdmin after selecting sweetbite_db.
-- This file only checks the tables; it does NOT change or delete any data.
SHOW CREATE TABLE products;
SHOW CREATE TABLE sales;
SELECT * FROM products;
SELECT * FROM sales;

-- For INSERT to work without entering IDs manually, ensure that
-- product_id and sale_id are INT PRIMARY KEY AUTO_INCREMENT.
-- Check the 'Extra' column in phpMyAdmin > Structure.
