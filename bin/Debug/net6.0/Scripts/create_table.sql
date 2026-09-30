-- Script de criação da tabela Produtos
-- Banco de dados: SQLite

CREATE TABLE IF NOT EXISTS Produtos (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome      TEXT    NOT NULL,
    Preco     REAL    NOT NULL,
    Estoque   INTEGER NOT NULL,
    Categoria TEXT    NOT NULL
);
