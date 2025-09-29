drop database if exists ga_studentblogg;
create database ga_studentblogg;
use ga_studentblogg;

# create user

CREATE USER IF NOT EXISTS 'ga-app'@'localhost' IDENTIFIED BY 'ga-5ecret-%';
CREATE USER IF NOT EXISTS 'ga-app'@'%' IDENTIFIED BY 'ga-5ecret-%';

GRANT ALL privileges ON ga_studentblogg.* TO 'ga-app'@'%';
GRANT ALL privileges ON ga_studentblogg.* TO 'ga-app'@'localhost';

FLUSH PRIVILEGES;