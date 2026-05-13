-- Create schemas for each microservice
-- This ensures logical isolation within a single PostgreSQL instance

CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS flights;
CREATE SCHEMA IF NOT EXISTS bookings;
CREATE SCHEMA IF NOT EXISTS admin;

-- Grant permissions
GRANT ALL PRIVILEGES ON SCHEMA identity TO admin;
GRANT ALL PRIVILEGES ON SCHEMA flights TO admin;
GRANT ALL PRIVILEGES ON SCHEMA bookings TO admin;
GRANT ALL PRIVILEGES ON SCHEMA admin TO admin;

-- Enable advisory locks for migration coordination
-- This is used by initContainer to prevent race conditions during migrations
