-- Schema and seed data for lift_pass.db (SQLite).
-- To rebuild the database:  sqlite3 lift_pass.db < initDatabase.sql

DROP TABLE IF EXISTS base_price;
DROP TABLE IF EXISTS holidays;

CREATE TABLE base_price (
    pass_id INTEGER PRIMARY KEY AUTOINCREMENT,
    type    TEXT    NOT NULL UNIQUE,
    cost    INTEGER NOT NULL
);

INSERT INTO base_price (type, cost) VALUES ('1jour', 35);
INSERT INTO base_price (type, cost) VALUES ('night', 19);

CREATE TABLE holidays (
    holiday     DATE NOT NULL PRIMARY KEY, -- stored as 'yyyy-MM-dd' text
    description TEXT NOT NULL
);

INSERT INTO holidays (holiday, description) VALUES
    ('2019-02-18', 'winter'),
    ('2019-02-25', 'winter'),
    ('2019-03-04', 'winter');
