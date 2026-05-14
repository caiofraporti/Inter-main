CREATE DATABASE MoveisCarrara
GO

USE MoveisCarrara
GO

-- ===========================
-- TABELA PESSOAS
-- ===========================
CREATE TABLE Pessoas (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    nome_social VARCHAR(100),
    tipo_pessoa char(1),
    documento VARCHAR(50),
    tipo_endereco VARCHAR(50),
    logradouro VARCHAR(100),
    numero VARCHAR(10),
    bairro VARCHAR(50),
    cidade VARCHAR(50),
    cep VARCHAR(20),
    telefone VARCHAR(20),
    email VARCHAR(100)
)
go

-- ===========================
-- CLIENTES
-- ===========================
CREATE TABLE Clientes (
    pessoa_id INT PRIMARY KEY,
    FOREIGN KEY (pessoa_id) REFERENCES Pessoas(id)
)
go

-- ===========================
-- FORNECEDORES
-- ===========================
CREATE TABLE Fornecedores (
    pessoa_id INT PRIMARY KEY,
    FOREIGN KEY (pessoa_id) REFERENCES Pessoas(id)
)
go

-- ===========================
-- USU�RIOS
-- ===========================
CREATE TABLE Funcionarios (
    pessoa_id INT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL,
    senha VARCHAR(200) NOT NULL,
    FOREIGN KEY (pessoa_id) REFERENCES Pessoas(id)
)
go

-- ===========================
-- TIPO_PRODUTOS
-- ===========================
CREATE TABLE Tipo_Produtos (
    codigo INT IDENTITY(1,1) PRIMARY KEY,
    nome_produto VARCHAR(100) NOT NULL,
    descricao VARCHAR(200),
    valor_unitario DECIMAL(10,2) NOT NULL
)
go

-- =========================================================
-- MATERIAIS
-- =========================================================

CREATE TABLE Materiais
(
    codigo INT IDENTITY(1,1) PRIMARY KEY,
    nome_material VARCHAR(100) NOT NULL,
    descricao VARCHAR(200),
    preco DECIMAL(10,2) NOT NULL
)
GO

-- ===========================
-- VENDAS
-- ===========================
CREATE TABLE Vendas (
    codigo INT IDENTITY(1,1) PRIMARY KEY,
    data DATE NOT NULL,
    total DECIMAL(10,2),
    nr_parcelas INT,
    cliente_id INT NOT NULL,
    funcionario_id INT NOT NULL,
    FOREIGN KEY (cliente_id) REFERENCES Clientes(pessoa_id),
    FOREIGN KEY (funcionario_id) REFERENCES Funcionarios(pessoa_id)
)
go

-- ===========================
-- COMPRAS
-- ===========================
CREATE TABLE Compras (
    codigo INT IDENTITY(1,1) PRIMARY KEY,
    data     DATE NOT NULL,
    total DECIMAL(10,2),
    nr_parcelas INT,
    fornecedor_id INT NOT NULL,
    funcionario_id INT NOT NULL,
    FOREIGN KEY (fornecedor_id) REFERENCES Fornecedores(pessoa_id),
    FOREIGN KEY (funcionario_id) REFERENCES Funcionarios(pessoa_id)
)


-- ===========================
-- SITUA��O
-- ===========================
CREATE TABLE Situacao (
    codigo INT IDENTITY(1,1) not null PRIMARY KEY,
    descricao VARCHAR(30) 
)
go


-- ===========================
-- LAN�AMENTOS
-- ===========================
CREATE TABLE Lancamentos (
    codigo INT IDENTITY(1,1) PRIMARY KEY,
    valor DECIMAL(10,2),
    parcela_nr varchar(10),
    data_vencimento DATE,
    data_pagamento DATE,
    venda_codigo INT NULL,
    compra_codigo INT NULL,
    situacao_codigo INT NULL,
    FOREIGN KEY (venda_codigo) REFERENCES Vendas(codigo),
    FOREIGN KEY (compra_codigo) REFERENCES Compras(codigo),
    FOREIGN KEY (situacao_codigo) REFERENCES Situacao(codigo)
)
go


-- =========================================================
-- RELACIONAMENTO N:N
-- VENDAS x TIPO_PRODUTOS
-- =========================================================

CREATE TABLE vendas_tipo_produtos
(
    venda_codigo INT NOT NULL,
    tipo_produto_codigo INT NOT NULL,

    item INT NOT NULL,
    qtd INT NOT NULL,
    preco DECIMAL(10,2) NOT NULL,
    dimensoes VARCHAR(20),

    PRIMARY KEY (venda_codigo, item),

    CONSTRAINT fk_vtp_venda
    FOREIGN KEY (venda_codigo)
    REFERENCES Vendas(codigo),

    CONSTRAINT fk_vtp_tipo_produto
    FOREIGN KEY (tipo_produto_codigo)
    REFERENCES Tipo_Produtos(codigo)
)
GO

-- =========================================================
-- RELACIONAMENTO N:N
-- COMPRAS x MATERIAIS
-- =========================================================

CREATE TABLE compras_materiais
(
    compra_codigo INT NOT NULL,
    material_codigo INT NOT NULL,

    item INT NOT NULL,
    qtd INT NOT NULL,
    preco DECIMAL(10,2) NOT NULL,

    PRIMARY KEY (compra_codigo, item),

    CONSTRAINT fk_cm_compra
    FOREIGN KEY (compra_codigo)
    REFERENCES Compras(codigo),

    CONSTRAINT fk_cm_material
    FOREIGN KEY (material_codigo)
    REFERENCES Materiais(codigo)
)
GO

INSERT INTO Situacao (descricao)
VALUES 
('Pendente'),
('Pago'),
('Atrasado');

INSERT INTO Lancamentos (valor, parcela_nr, data_vencimento, data_pagamento, venda_codigo, compra_codigo, situacao_codigo)
VALUES 
(400.00, '1/2', '2026-04-01', NULL, 1, NULL, 1),
(500.00, '1/3', '2026-04-05', '2026-04-05', 2, NULL, 2),
(1500.00, '1/1', '2026-04-10', NULL, NULL, 1, 1);

INSERT INTO vendas_produtos (venda_codigo, item, produto_codigo, qtd, preco, dimensoes)
VALUES 
(1, 1, 1, 1, 500.00, '100x60'),
(2, 1, 3, 1, 1200.00, '200x90'),
(3, 1, 2, 4, 150.00, '40x40');
go
-- =========================================================================================
-- VIEWS
--==========================================================================================
CREATE VIEW v_clientes
AS
SELECT c.pessoa_id codigoCliente, p.nome, p.cep, p.logradouro, p.cidade, p.documento, 
case p.tipo_pessoa
    when 'F' then 'Pessoa fisica'
    when 'J' then 'Pessoa juridica'
    end tipo_pessoa
FROM Clientes c, Pessoas p
where c.pessoa_id = p.id
GO

CREATE VIEW v_fornecedores
as
select f.pessoa_id, p.nome, p.cep, p.logradouro,p.cidade, p.email, p.telefone, p.documento,
case p.tipo_pessoa
    when 'F' then 'Pessoa fisica'
    when 'J' then 'pessoa juridica'
    end tipo_pessoa
from Fornecedores f, Pessoas p
where f.pessoa_id = p.id
go

CREATE VIEW v_funcionarios
as
select u.pessoa_id, p.nome, u.usuario, u.senha, p.telefone, p.email, p.documento
from Funcionarios u, pessoas p
where u.pessoa_id = p.id
go

CREATE VIEW v_produtos
AS
SELECT codigo, nome_produto, valor_unitario
FROM Produtos
GO

CREATE VIEW v_vendas
AS
SELECT v.codigo, v.data, v.total, pc.nome cliente, pf.nome funcionario
FROM Vendas v, Pessoas pc, Pessoas pf 
where pc.id = v.cliente_id and
      pf.id = v.funcionario_id
GO

CREATE VIEW v_compras
AS
SELECT c.codigo, c.data, c.total, pf.nome fornecedor, pu.nome funcionario
FROM Compras c, Pessoas pf, Pessoas pu
where pf.id = c.fornecedor_id and
      pu.id = c.funcionario_id
GO

CREATE VIEW v_situacao
AS
SELECT codigo, descricao
FROM Situacao
GO

CREATE VIEW v_lancamentos
AS
SELECT codigo, valor, data_vencimento, data_pagamento
FROM Lancamentos
GO

CREATE VIEW v_vendas_produtos
AS
SELECT venda_codigo, produto_codigo, qtd, preco
FROM vendas_produtos
GO

--==============================================================================================
-- 7 VIEWS COM JOINS
--==============================================================================================

CREATE VIEW v_vendas_financeiro
AS
SELECT v.codigo,
       p.nome cliente,
       v.data,
       v.nr_parcelas,
       v.total,
       CAST(v.total /v.nr_parcelas AS decimal(10,2)) valor_parcela
FROM Vendas v, Pessoas p
WHERE v.cliente_id = p.id
GO

CREATE VIEW v_compras_financeiro
AS
SELECT c.codigo,
       p.nome fornecedor,
       c.data,
       c.nr_parcelas,
       c.total,
       CAST(c.total/c.nr_parcelas as decimal(10,2)) valor_parcela
FROM Compras c, Pessoas p
WHERE c.fornecedor_id = p.id
GO

CREATE VIEW v_itens_venda_produto
AS
SELECT vp.venda_codigo,
       pr.nome_produto,
       vp.qtd,
       vp.preco
FROM vendas_produtos vp, Produtos pr
WHERE vp.produto_codigo = pr.codigo
GO



CREATE VIEW v_vendas_completa
AS
SELECT v.codigo,
       v.data,
       pc.nome cliente,
       pf.nome funcionario,
       v.total
FROM Vendas v, Pessoas pc, Pessoas pf
WHERE v.cliente_id = pc.id
  AND v.funcionario_id = pf.id
GO

CREATE VIEW v_lancamentos_completo
AS
SELECT l.codigo,
       l.valor,
       s.descricao situacao,
       l.data_vencimento,
       l.data_pagamento
FROM Lancamentos l, Situacao s
WHERE l.situacao_codigo = s.codigo
GO


CREATE VIEW v_venda_produtos_detalhado
AS
SELECT v.codigo venda,
       pr.nome_produto,
       vp.qtd,
       vp.preco
FROM Vendas v, vendas_produtos vp, Produtos pr
WHERE v.codigo = vp.venda_codigo
  AND vp.produto_codigo = pr.codigo
GO

CREATE VIEW v_parcelas_pagas
AS
SELECT l.codigo,
       l.parcela_nr,
       l.valor,
       l.data_pagamento
FROM Lancamentos l
WHERE l.data_pagamento IS NOT NULL
GO

--===========================================================================================
-- FUN��ES ESCALARES
--===========================================================================================
CREATE FUNCTION getValorParcela
(
    @total DECIMAL(10,2),
    @parcelas INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN (
        CASE 
            WHEN @parcelas = 0 THEN 0
            ELSE CAST(@total * 1.0 / @parcelas AS DECIMAL(10,2))
        END
    )
END
GO

SELECT dbo.getValorParcela(1500, 3) ValorParcela
go

CREATE FUNCTION getTotalVenda
(
    @venda INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN (
        SELECT SUM(qtd * preco)
        FROM vendas_produtos
        WHERE venda_codigo = @venda
    )
END
GO

SELECT dbo.getTotalVenda(1) Total
go

CREATE FUNCTION getTotalPagoVenda
(
    @venda INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN (
        SELECT SUM(valor)
        FROM Lancamentos
        WHERE venda_codigo = @venda
          AND data_pagamento IS NOT NULL
    )
END
GO

SELECT dbo.getTotalPagoVenda(1) Pago
go
--=========================================================================================================================
-- FUN��ES TABLE - TIPO 1
--=========================================================================================================================
CREATE FUNCTION getVendasFunc (@funcionario INT)
RETURNS TABLE
AS
RETURN (
    SELECT v.codigo,
           v.data,
           p.nome cliente,
           v.total
    FROM Vendas v, Pessoas p
    WHERE v.cliente_id = p.id
      AND v.funcionario_id = @funcionario
)
GO

SELECT * FROM getVendasFunc(1)
SELECT SUM(total) Total FROM getVendasFunc(1)
go

CREATE FUNCTION getProdData (@data DATE)
RETURNS TABLE
AS
RETURN (
    SELECT pr.codigo,
           pr.nome_produto,
           SUM(vp.qtd) qtd_total
    FROM Vendas v, vendas_produtos vp, Produtos pr
    WHERE v.codigo = vp.venda_codigo
      AND vp.produto_codigo = pr.codigo
      AND v.data = @data
    GROUP BY pr.codigo, pr.nome_produto
)
GO

SELECT * FROM getProdData('2026-03-01')
go

CREATE FUNCTION getLancVenda (@venda INT)
RETURNS TABLE
AS
RETURN (
    SELECT l.codigo,
           l.parcela_nr,
           l.valor,
           l.data_vencimento,
           l.data_pagamento,
           s.descricao situacao
    FROM Lancamentos l, Situacao s
    WHERE l.situacao_codigo = s.codigo
      AND l.venda_codigo = @venda
)
GO

SELECT * FROM getLancVenda(1)
SELECT SUM(valor) Total FROM getLancVenda(1)
go
--==========================================================================================
-- PROCEDURES
--==========================================================================================

CREATE PROCEDURE sp_insert_pessoas
(
    @nome VARCHAR(100),
    @nome_social VARCHAR(100),
    @tipo_pessoa CHAR(1),
    @documento VARCHAR(50),
    @tipo_endereco VARCHAR(50),
    @logradouro VARCHAR(100),
    @numero VARCHAR(10),
    @bairro VARCHAR(50),
    @cidade VARCHAR(50),
    @cep VARCHAR(20),
    @telefone VARCHAR(20),
    @email VARCHAR(100)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			if not exists (select * from Pessoas where documento = @documento and email = @email and telefone = @telefone)
				INSERT INTO Pessoas
				VALUES
				(@nome, @nome_social, @tipo_pessoa, @documento, @tipo_endereco,
				@logradouro, @numero, @bairro, @cidade, @cep, @telefone, @email)
		COMMIT
		 RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		if exists (select * from Pessoas where documento = @documento)
			PRINT 'Documento j� cadastrado'
		if exists (select * from Pessoas where email = @email)
			PRINT 'Email j� cadastrado'
		if exists (select * from Pessoas where telefone = @telefone)
			PRINT 'Telefone j� cadastrado'
		RETURN 1
	END CATCH
END
GO

CREATE PROCEDURE sp_update_pessoas
(
    @id INT,
    @nome VARCHAR(100),
    @nome_social VARCHAR(100),
    @tipo_pessoa CHAR(1),
    @documento VARCHAR(50),
    @tipo_endereco VARCHAR(50),
    @logradouro VARCHAR(100),
    @numero VARCHAR(10),
    @bairro VARCHAR(50),
    @cidade VARCHAR(50),
    @cep VARCHAR(20),
    @telefone VARCHAR(20),
    @email VARCHAR(100)
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			if not exists (select * from Pessoas where documento = @documento and email = @email and telefone = @telefone)
				UPDATE Pessoas SET
					nome = @nome,
					nome_social = @nome_social,
					tipo_pessoa = @tipo_pessoa,
					documento = @documento,
					tipo_endereco = @tipo_endereco,
					logradouro = @logradouro,
					numero = @numero,
					bairro = @bairro,
					cidade = @cidade,
					cep = @cep,
					telefone = @telefone,
					email = @email
		WHERE id = @id
		COMMIT
		RETURN 0
	END TRY
	BEGIN CATCH
		ROLLBACK
		if exists (select * from Pessoas where documento = @documento)
			PRINT 'Documento j� cadastrado'
		if exists (select * from Pessoas where email = @email)
			PRINT 'Email j� cadastrado'
		if exists (select * from Pessoas where telefone = @telefone)
			PRINT 'Telefone j� cadastrado'	
		RETURN 1
	END CATCH
END
GO








CREATE PROCEDURE sp_insert_clientes
(@pessoa_id INT)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			if not exists (select * from Clientes where pessoa_id = @pessoa_id)
				INSERT INTO Clientes VALUES (@pessoa_id)
		COMMIT
		RETURN 0 
	END TRY
	BEGIN CATCH
		ROLLBACK
		PRINT 'Cliente j� cadastrado'
	END CATCH
END
GO

CREATE PROCEDURE sp_update_clientes
(@pessoa_id INT, @novo_id INT)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Clientes WHERE pessoa_id = @pessoa_id)
        BEGIN
            PRINT 'Cliente n�o encontrado'
            RETURN 1
        END

        IF EXISTS (SELECT 1 FROM Clientes WHERE pessoa_id = @novo_id)
        BEGIN
            PRINT 'Novo ID j� est� vinculado a outro cliente'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Pessoas WHERE id = @novo_id)
        BEGIN
            PRINT 'Pessoa n�o existe'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Clientes
        SET pessoa_id = @novo_id
        WHERE pessoa_id = @pessoa_id

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar cliente'
        RETURN 1
    END CATCH
END
GO













CREATE PROCEDURE sp_insert_fornecedores
(@pessoa_id INT)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Pessoas WHERE id = @pessoa_id)
        BEGIN
            PRINT 'Pessoa n�o existe'
            RETURN 1
        END

        IF EXISTS (SELECT 1 FROM Fornecedores WHERE pessoa_id = @pessoa_id)
        BEGIN
            PRINT 'Fornecedor j� cadastrado'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Fornecedores VALUES (@pessoa_id)

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir fornecedor'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_fornecedores
(@pessoa_id INT, @novo_id INT)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Fornecedores WHERE pessoa_id = @pessoa_id)
        BEGIN
            PRINT 'Fornecedor n�o encontrado'
            RETURN 1
        END

        IF EXISTS (SELECT 1 FROM Fornecedores WHERE pessoa_id = @novo_id)
        BEGIN
            PRINT 'Novo ID j� est� em uso'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Fornecedores
        SET pessoa_id = @novo_id
        WHERE pessoa_id = @pessoa_id

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar fornecedor'
        RETURN 1
    END CATCH
END
GO















CREATE PROCEDURE sp_insert_produtos
(
    @nome VARCHAR(100),
    @descricao VARCHAR(200),
    @valor DECIMAL(10,2)
)
AS
BEGIN
    BEGIN TRY

        IF @valor <= 0
        BEGIN
            PRINT 'Valor inv�lido'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Produtos VALUES (@nome, @descricao, @valor)

        COMMIT

        SELECT SCOPE_IDENTITY() AS codigo
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir produto'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_produtos
(
    @codigo INT,
    @nome VARCHAR(100),
    @descricao VARCHAR(200),
    @valor DECIMAL(10,2)
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Produtos WHERE codigo = @codigo)
        BEGIN
            PRINT 'Produto n�o encontrado'
            RETURN 1
        END

        IF @valor <= 0
        BEGIN
            PRINT 'Valor inv�lido'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Produtos SET
            nome_produto = @nome,
            descricao = @descricao,
            valor_unitario = @valor
        WHERE codigo = @codigo

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar produto'
        RETURN 1
    END CATCH
END
GO












CREATE PROCEDURE sp_insert_vendas
(
    @data DATE,
    @total DECIMAL(10,2),
    @parcelas INT,
    @cliente INT,
    @funcionario INT
)
AS
BEGIN
    BEGIN TRY

        -- VALIDA��ES
        IF @total <= 0
        BEGIN
            PRINT 'Total inv�lido'
            RETURN 1
        END

        IF @parcelas <= 0
        BEGIN
            PRINT 'N�mero de parcelas inv�lido'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Clientes WHERE pessoa_id = @cliente)
        BEGIN
            PRINT 'Cliente n�o existe'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Funcionarios WHERE pessoa_id = @funcionario)
        BEGIN
            PRINT 'Funcion�rio n�o existe'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Vendas
        VALUES (@data, @total, @parcelas, @cliente, @funcionario)

        COMMIT

        SELECT SCOPE_IDENTITY() AS codigo
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir venda'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_vendas
(
    @codigo INT,
    @data DATE,
    @total DECIMAL(10,2),
    @parcelas INT,
    @cliente INT,
    @funcionario INT
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Vendas WHERE codigo = @codigo)
        BEGIN
            PRINT 'Venda n�o encontrada'
            RETURN 1
        END

        IF @total <= 0
        BEGIN
            PRINT 'Total inv�lido'
            RETURN 1
        END

        IF @parcelas <= 0
        BEGIN
            PRINT 'Parcelas inv�lidas'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Clientes WHERE pessoa_id = @cliente)
        BEGIN
            PRINT 'Cliente n�o existe'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Funcionarios WHERE pessoa_id = @funcionario)
        BEGIN
            PRINT 'Funcion�rio n�o existe'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Vendas SET
            data = @data,
            total = @total,
            nr_parcelas = @parcelas,
            cliente_id = @cliente,
            funcionario_id = @funcionario
        WHERE codigo = @codigo

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar venda'
        RETURN 1
    END CATCH
END
GO















CREATE PROCEDURE sp_insert_compras
(
    @data DATE,
    @total DECIMAL(10,2),
    @parcelas INT,
    @fornecedor INT,
    @funcionario INT
)
AS
BEGIN
    BEGIN TRY

        IF @total <= 0
        BEGIN
            PRINT 'Total inv�lido'
            RETURN 1
        END

        IF @parcelas <= 0
        BEGIN
            PRINT 'Parcelas inv�lidas'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Fornecedores WHERE pessoa_id = @fornecedor)
        BEGIN
            PRINT 'Fornecedor n�o existe'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Funcionarios WHERE pessoa_id = @funcionario)
        BEGIN
            PRINT 'Funcion�rio n�o existe'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Compras
        VALUES (@data, @total, @parcelas, @fornecedor, @funcionario)

        COMMIT

        SELECT SCOPE_IDENTITY() AS codigo
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir compra'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_compras
(
    @codigo INT,
    @data DATE,
    @total DECIMAL(10,2),
    @parcelas INT,
    @fornecedor INT,
    @funcionario INT
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Compras WHERE codigo = @codigo)
        BEGIN
            PRINT 'Compra n�o encontrada'
            RETURN 1
        END

        IF @total <= 0
        BEGIN
            PRINT 'Total inv�lido'
            RETURN 1
        END

        IF @parcelas <= 0
        BEGIN
            PRINT 'Parcelas inv�lidas'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Compras SET
            data = @data,
            total = @total,
            nr_parcelas = @parcelas,
            fornecedor_id = @fornecedor,
            funcionario_id = @funcionario
        WHERE codigo = @codigo

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar compra'
        RETURN 1
    END CATCH
END
GO














CREATE PROCEDURE sp_insert_situacao
(@descricao VARCHAR(30))
AS
BEGIN
    BEGIN TRY

        IF @descricao IS NULL OR LTRIM(RTRIM(@descricao)) = ''
        BEGIN
            PRINT 'Descri��o obrigat�ria'
            RETURN 1
        END

        IF EXISTS (SELECT 1 FROM Situacao WHERE descricao = @descricao)
        BEGIN
            PRINT 'Situa��o j� existe'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Situacao VALUES (@descricao)

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir situa��o'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_situacao
(
    @codigo INT,
    @descricao VARCHAR(30)
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Situacao WHERE codigo = @codigo)
        BEGIN
            PRINT 'Situa��o n�o encontrada'
            RETURN 1
        END

        IF @descricao IS NULL OR LTRIM(RTRIM(@descricao)) = ''
        BEGIN
            PRINT 'Descri��o obrigat�ria'
            RETURN 1
        END

        IF EXISTS (SELECT 1 FROM Situacao 
                   WHERE descricao = @descricao AND codigo <> @codigo)
        BEGIN
            PRINT 'J� existe uma situa��o com essa descri��o'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Situacao
        SET descricao = @descricao
        WHERE codigo = @codigo

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar situa��o'
        RETURN 1
    END CATCH
END
GO
















CREATE PROCEDURE sp_insert_lancamentos
(
    @valor DECIMAL(10,2),
    @parcela VARCHAR(10),
    @vencimento DATE,
    @pagamento DATE = NULL,
    @venda INT = NULL,
    @compra INT = NULL,
    @situacao INT = NULL
)
AS
BEGIN
    BEGIN TRY

        IF @valor <= 0
        BEGIN
            PRINT 'Valor inv�lido'
            RETURN 1
        END

        IF @venda IS NULL AND @compra IS NULL
        BEGIN
            PRINT 'Deve informar venda ou compra'
            RETURN 1
        END

        IF @venda IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Vendas WHERE codigo = @venda)
        BEGIN
            PRINT 'Venda n�o existe'
            RETURN 1
        END

        IF @compra IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Compras WHERE codigo = @compra)
        BEGIN
            PRINT 'Compra n�o existe'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO Lancamentos
        VALUES (@valor, @parcela, @vencimento, @pagamento, @venda, @compra, @situacao)

        COMMIT

        SELECT SCOPE_IDENTITY() AS codigo
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir lan�amento'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_lancamentos
(
    @codigo INT,
    @valor DECIMAL(10,2),
    @parcela VARCHAR(10),
    @vencimento DATE,
    @pagamento DATE,
    @venda INT,
    @compra INT,
    @situacao INT
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Lancamentos WHERE codigo = @codigo)
        BEGIN
            PRINT 'Lan�amento n�o encontrado'
            RETURN 1
        END

        IF @valor <= 0
        BEGIN
            PRINT 'Valor inv�lido'
            RETURN 1
        END

        IF @venda IS NULL AND @compra IS NULL
        BEGIN
            PRINT 'Informe venda ou compra'
            RETURN 1
        END

        IF @venda IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Vendas WHERE codigo = @venda)
        BEGIN
            PRINT 'Venda n�o existe'
            RETURN 1
        END

        IF @compra IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Compras WHERE codigo = @compra)
        BEGIN
            PRINT 'Compra n�o existe'
            RETURN 1
        END

        IF @situacao IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Situacao WHERE codigo = @situacao)
        BEGIN
            PRINT 'Situa��o n�o existe'
            RETURN 1
        END

        IF @pagamento IS NOT NULL AND @pagamento < @vencimento
        BEGIN
            PRINT 'Data de pagamento n�o pode ser anterior ao vencimento'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE Lancamentos SET
            valor = @valor,
            parcela_nr = @parcela,
            data_vencimento = @vencimento,
            data_pagamento = @pagamento,
            venda_codigo = @venda,
            compra_codigo = @compra,
            situacao_codigo = @situacao
        WHERE codigo = @codigo

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar lan�amento'
        RETURN 1
    END CATCH
END
GO

















CREATE PROCEDURE sp_insert_vendas_produtos
(
    @venda INT,
    @item INT,
    @produto INT,
    @qtd INT,
    @preco DECIMAL(10,2),
    @dimensoes VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM Vendas WHERE codigo = @venda)
        BEGIN
            PRINT 'Venda n�o existe'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Produtos WHERE codigo = @produto)
        BEGIN
            PRINT 'Produto n�o existe'
            RETURN 1
        END

        IF @qtd <= 0
        BEGIN
            PRINT 'Quantidade inv�lida'
            RETURN 1
        END

        BEGIN TRAN

        INSERT INTO vendas_produtos
        VALUES (@venda, @item, @produto, @qtd, @preco, @dimensoes)

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao inserir item da venda'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_update_vendas_produtos
(
    @venda INT,
    @item INT,
    @produto INT,
    @qtd INT,
    @preco DECIMAL(10,2),
    @dimensoes VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY

        IF NOT EXISTS (
            SELECT 1 FROM vendas_produtos 
            WHERE venda_codigo = @venda AND item = @item
        )
        BEGIN
            PRINT 'Item da venda n�o encontrado'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Vendas WHERE codigo = @venda)
        BEGIN
            PRINT 'Venda n�o existe'
            RETURN 1
        END

        IF NOT EXISTS (SELECT 1 FROM Produtos WHERE codigo = @produto)
        BEGIN
            PRINT 'Produto n�o existe'
            RETURN 1
        END

        IF @qtd <= 0
        BEGIN
            PRINT 'Quantidade inv�lida'
            RETURN 1
        END

        IF @preco <= 0
        BEGIN
            PRINT 'Pre�o inv�lido'
            RETURN 1
        END

        BEGIN TRAN

        UPDATE vendas_produtos SET
            produto_codigo = @produto,
            qtd = @qtd,
            preco = @preco,
            dimensoes = @dimensoes
        WHERE venda_codigo = @venda
          AND item = @item

        COMMIT
        RETURN 0

    END TRY
    BEGIN CATCH
        ROLLBACK
        PRINT 'Erro ao atualizar item da venda'
        RETURN 1
    END CATCH
END
GO

CREATE PROCEDURE sp_insert_funcionarios
(
    @nome VARCHAR(100),
    @nome_social VARCHAR(100),
    @tipo_pessoa CHAR(1),
    @documento VARCHAR(50),
    @tipo_endereco VARCHAR(50),
    @logradouro VARCHAR(100),
    @numero VARCHAR(10),
    @bairro VARCHAR(50),
    @cidade VARCHAR(50),
    @cep VARCHAR(20),
    @telefone VARCHAR(20),
    @email VARCHAR(100),
    @usuario VARCHAR(50),
    @senha VARCHAR(200)
)
AS
BEGIN

    BEGIN TRY

        BEGIN TRAN

        INSERT INTO Pessoas
        (
            nome,
            nome_social,
            tipo_pessoa,
            documento,
            tipo_endereco,
            logradouro,
            numero,
            bairro,
            cidade,
            cep,
            telefone,
            email
        )
        VALUES
        (
            @nome,
            @nome_social,
            @tipo_pessoa,
            @documento,
            @tipo_endereco,
            @logradouro,
            @numero,
            @bairro,
            @cidade,
            @cep,
            @telefone,
            @email
        )

        DECLARE @pessoa_id INT

        SET @pessoa_id = SCOPE_IDENTITY()

        INSERT INTO Funcionarios
        (
            pessoa_id,
            usuario,
            senha
        )
        VALUES
        (
            @pessoa_id,
            @usuario,
            @senha
        )

        COMMIT

    END TRY

    BEGIN CATCH

        ROLLBACK

    END CATCH

END
GO

EXEC sp_insert_funcionarios
    'Administrador',
    NULL,
    'F',
    '11111111111',
    'Residencial',
    'Rua A',
    '100',
    'Centro',
    'Rio Preto',
    '15000-000',
    '17999999999',
    'admin@admin.com',
    'admin',
    '123456'
go


