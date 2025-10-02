CREATE DATABASE metin2;

CREATE USER metin_app_user WITH PASSWORD 'm3t!n' NOINHERIT;
COMMENT ON ROLE metin_app_user IS 'Backend app user';

CREATE USER metin_migrations_user WITH PASSWORD 'm3t!n_m!gr@ti0ns' NOINHERIT;
COMMENT ON ROLE metin_migrations_user IS 'Backend migrations user';

GRANT CREATE ON DATABASE metin2 TO metin_migrations_user;

\c metin2;
GRANT USAGE, CREATE ON SCHEMA public TO metin_migrations_user;
GRANT USAGE ON SCHEMA public TO metin_app_user;
