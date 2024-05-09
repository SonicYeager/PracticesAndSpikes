CREATE DATABASE simpleGroupBy;
USE simpleGroupBy;

CREATE TABLE people (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        name VARCHAR(255),
                        age INT
);

INSERT INTO people (name, age)
VALUES
    ('John', 25),
    ('Jane', 30),
    ('Alice', 25),
    ('Bob', 40),
    ('Charlie', 30),
    ('Dave', 40),
    ('Eve', 25);