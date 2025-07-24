-- Tabela de Usuários
CREATE TABLE AppUsers (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User'
);

-- Tabela de Preferências de Tópicos do Usuário
CREATE TABLE UserTopicPreferences (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Topic NVARCHAR(100) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES AppUsers(UserId)
);

-- Índice para consultas de preferência
CREATE INDEX IX_UserTopicPreferences_UserId ON UserTopicPreferences(UserId);

-- Criar um usuário administrador padrão (senha: Admin@123)
INSERT INTO AppUsers (UserId, Email, Name, Password, Role)
VALUES (
    NEWID(), 
    'admin@example.com', 
    'Administrador', 
    -- Nota: Em produção, use um hash adequado em vez de senha em texto puro
    'AQAAAAEAACcQAAAAEBIoFdlczQaDwfSYrI6DjKkhVU4P5ZHfDnGNhrxu3Y6ehhVl01L+3W+Fe5S7zHZhGg==', 
    'Admin'
);
