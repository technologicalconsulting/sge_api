-- Crear la base de datos (si no existe)
CREATE DATABASE SGE;

-- Tabla empresa
CREATE TABLE empresa (
    id SERIAL PRIMARY KEY,
    razon_social VARCHAR(255) NOT NULL,
    nombre_comercial VARCHAR(255),
    ruc VARCHAR(20) UNIQUE NOT NULL,
    tipo_empresa VARCHAR(50) CHECK (tipo_empresa IN (
        'Sociedad Anónima (SA)', 
        'Sociedad por Acciones Simplificada (SAS)', 
        'Compañía de Responsabilidad Limitada (Cía. Ltda.)', 
        'Compañía en Nombre Colectivo', 
        'Compañía en Comandita Simple', 
        'Compañía en Comandita por Acciones'
    )) NOT NULL,
    sector VARCHAR(100),
    direccion TEXT NOT NULL,
    ciudad VARCHAR(100),
    pais VARCHAR(100) NOT NULL,
    telefono VARCHAR(20),
    email VARCHAR(255) UNIQUE NOT NULL,
    sitio_web VARCHAR(255),
    logo_url VARCHAR(255),
    fecha_fundacion DATE,
    estado VARCHAR(50) CHECK (estado IN ('Activo', 'Inactivo')) DEFAULT 'Activo',
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    notas TEXT
);


-- Tabla clientes con relación a empresa
CREATE TABLE clientes (
    id SERIAL PRIMARY KEY,
    empresa_id INTEGER REFERENCES empresa(id) ON DELETE CASCADE,
    razon_social VARCHAR(255) NOT NULL,
    nombre_comercial VARCHAR(255),
    ruc VARCHAR(20) UNIQUE NOT NULL,
    tipo_empresa VARCHAR(50) CHECK (tipo_empresa IN (
        'Sociedad Anónima (SA)', 
        'Sociedad por Acciones Simplificada (SAS)', 
        'Compañía de Responsabilidad Limitada (Cía. Ltda.)', 
        'Compañía en Nombre Colectivo', 
        'Compañía en Comandita Simple', 
        'Compañía en Comandita por Acciones'
    )) NOT NULL,
    sector VARCHAR(100),
    direccion TEXT NOT NULL,
    ciudad VARCHAR(100),
    pais VARCHAR(100) NOT NULL,
    sitio_web VARCHAR(255),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    estado VARCHAR(50) CHECK (estado IN ('Activo', 'Inactivo', 'Suspendido')) DEFAULT 'Activo',
    limite_credito DECIMAL(15,2) DEFAULT 0.00,
    notas TEXT
);

-- Tabla contactos_clientes para múltiples contactos por cliente
CREATE TABLE contactos_clientes (
    id SERIAL PRIMARY KEY,
    cliente_id INTEGER REFERENCES clientes(id) ON DELETE CASCADE,
    nombre VARCHAR(255) NOT NULL,
    cargo VARCHAR(100),
    telefono VARCHAR(20),
    email VARCHAR(255) UNIQUE NOT NULL,
    notas TEXT
);

CREATE TABLE empleado (
    id SERIAL PRIMARY KEY,
    empresa_id INTEGER REFERENCES empresa(id) ON DELETE CASCADE,
    primer_nombre VARCHAR(100) NOT NULL,
    segundo_nombre VARCHAR(100), -- Puede ser NULL si no tiene segundo nombre
    apellido_paterno VARCHAR(100) NOT NULL,
    apellido_materno VARCHAR(100), -- Puede ser NULL si no tiene apellido materno
    tipo_documento VARCHAR(50) CHECK (tipo_documento IN ('Cédula', 'Pasaporte', 'Otro')) NOT NULL, -- Nuevo campo
    numero_identificacion VARCHAR(20) UNIQUE NOT NULL, 
    email_personal VARCHAR(255) UNIQUE NOT NULL, -- Nuevo campo
    email_corporativo VARCHAR(255) UNIQUE, -- Nuevo campo, puede ser NULL
    telefono VARCHAR(20),
    direccion TEXT,
    fecha_nacimiento DATE,
    genero VARCHAR(20) CHECK (genero IN ('Masculino', 'Femenino', 'Otro')),
    estado VARCHAR(50) CHECK (estado IN ('Activo', 'Inactivo', 'Suspendido', 'Eliminado')) DEFAULT 'Activo',
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);-- Añadir la columna representante_legal_id
ALTER TABLE empresa 
ADD COLUMN representante_legal_id INTEGER UNIQUE REFERENCES empleado(id) ON DELETE SET NULL;

-- Tabla intermedia empleado-cliente para asignaciones múltiples
CREATE TABLE empleado_cliente (
    id SERIAL PRIMARY KEY,
    empleado_id INTEGER REFERENCES empleado(id) ON DELETE CASCADE,
    cliente_id INTEGER REFERENCES clientes(id) ON DELETE CASCADE,
    fecha_asignacion DATE DEFAULT CURRENT_DATE,
    fecha_fin DATE NULL,
    estado VARCHAR(20) CHECK (estado IN ('Activo', 'Finalizado', 'Pendiente')) DEFAULT 'Activo'
);


-- Listado de Departamentos
CREATE TABLE departamentos (
    id SERIAL PRIMARY KEY, 
    empresa_id INTEGER REFERENCES empresa(id) ON DELETE CASCADE, 
    nombre VARCHAR(100) UNIQUE NOT NULL, 
    descripcion TEXT 
);

-- Listado de Cargos
CREATE TABLE cargos (
    id SERIAL PRIMARY KEY, 
    empresa_id INTEGER REFERENCES empresa(id) ON DELETE CASCADE, 
    nombre VARCHAR(100) NOT NULL,  
    descripcion TEXT,              
    departamento_id INTEGER REFERENCES departamentos(id) ON DELETE SET NULL, 
    UNIQUE(nombre, empresa_id)  
);

-- Información Laboral del Usuario
CREATE TABLE informacion_laboral_empleado (
    id SERIAL PRIMARY KEY,
    empleado_id INTEGER REFERENCES empleado(id) ON DELETE CASCADE, -- Empleado al que se le asigna la información laboral
    empresa_id INTEGER REFERENCES empresa(id) ON DELETE CASCADE, -- Empresa donde trabaja
    departamento_id INTEGER REFERENCES departamentos(id) ON DELETE SET NULL, -- Departamento (si aplica)
    cargo_id INTEGER REFERENCES cargos(id) ON DELETE SET NULL, -- Cargo dentro de la empresa
    fecha_ingreso DATE NOT NULL, -- Fecha de ingreso
    fecha_salida DATE, -- Fecha de salida (si aplica)
    salario DECIMAL(10,2), -- Salario del empleado
    tipo_contrato VARCHAR(50) CHECK (tipo_contrato IN ('Indefinido', 'Temporal', 'Pasante', 'Contrato por obra', 'Freelance')),
    
    -- Supervisor Interno (Empleado de la Empresa)
    supervisor_interno_id INTEGER REFERENCES empleado(id) ON DELETE SET NULL,
    
    -- Supervisor Externo (Contacto del Cliente)
    supervisor_externo_id INTEGER REFERENCES contactos_clientes(id) ON DELETE SET NULL,

    notas TEXT -- Campo para observaciones adicionales
);

-- Tabla historial_sesiones
CREATE TABLE historial_sesiones (
    id SERIAL PRIMARY KEY,
    empleado_id INTEGER REFERENCES empleado(id) ON DELETE CASCADE,
    ip VARCHAR(45),
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    navegador VARCHAR(255),
    exito BOOLEAN DEFAULT TRUE
);

-- Tabla de User del Sistema (Autenticación)
CREATE TABLE users(
    id SERIAL PRIMARY KEY,
    empleado_id INTEGER UNIQUE REFERENCES empleado(id) ON DELETE CASCADE,
    numero_identificacion VARCHAR(20) UNIQUE NOT NULL,
    usuario VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    intentos_fallidos INTEGER DEFAULT 0,
    bloqueado BOOLEAN DEFAULT FALSE,
    estado VARCHAR(20) CHECK (estado IN ('Activo', 'Inactivo', 'Bloqueado')) DEFAULT 'Activo',
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_ultimo_login TIMESTAMP DEFAULT NULL
);

-- Tabla para códigos de validación de registro y recuperación de contraseña
CREATE TABLE codigos_verificacion (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    codigo VARCHAR(6) NOT NULL,
    tipo VARCHAR(20) CHECK (tipo IN ('Registro', 'Recuperacion')) NOT NULL,
    fecha_generacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expiracion TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE
);

-- Historial de intentos de acceso
CREATE TABLE historial_intentos (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    exito BOOLEAN NOT NULL,
    ip VARCHAR(45),
    navegador VARCHAR(255)
);

-- Historial de bloqueos de user
CREATE TABLE historial_bloqueos (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    fecha_bloqueo TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    razon TEXT NOT NULL,
    motivo_bloqueo TEXT
);

-- Historial de recuperación de contraseñas
CREATE TABLE historial_recuperacion (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_cambio TIMESTAMP DEFAULT NULL,
    exito BOOLEAN DEFAULT FALSE
);

-- Tabla de Roles del Sistema
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL,  -- Ej: Administrador, Usuario, Supervisor
    descripcion TEXT
);


-- Relación de User con Roles
CREATE TABLE users_roles (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    rol_id INTEGER REFERENCES roles(id) ON DELETE CASCADE,
    UNIQUE(usuario_id, rol_id)
);

-- Tabla de Permisos del Sistema
CREATE TABLE permisos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) UNIQUE NOT NULL,
    descripcion TEXT
);

-- Relación de Permisos con Roles
CREATE TABLE roles_permisos (
    id SERIAL PRIMARY KEY,
    rol_id INTEGER REFERENCES roles(id) ON DELETE CASCADE,
    permiso_id INTEGER REFERENCES permisos(id) ON DELETE CASCADE,
    UNIQUE(rol_id, permiso_id)
);
 
 
INSERT INTO empresa (
    razon_social, nombre_comercial, ruc, tipo_empresa, sector, direccion, ciudad, pais, telefono, email, 
    sitio_web, logo_url, fecha_fundacion, estado, notas
) VALUES 
(
    'TCR TECHNOLOGICAL CONSULTING & RISK S.A.S.', 'TECHNOLOGICAL CONSULTING', '0993392436001', 
    'Sociedad por Acciones Simplificada (SAS)', 'Servicios Tecnológicos', 
    'Av. 9 de Octubre 123, Guayaquil', 'Guayaquil', 'Ecuador', '0963973280', 'info@technologicalconsulting.com', 
    'https://technologicalconsulting.com', 'https://technologicalconsulting.com/logo.png', 
    '2015-07-31', 'Activo', 'Empresa líder en seguridad informática'
),
(
    'Innovatech S.A.', 'Innovatech', '099987654321', 
    'Compañía de Responsabilidad Limitada (Cía. Ltda.)', 'Desarrollo de Software', 
    'Av. Amazonas 456, Quito', 'Quito', 'Ecuador', '0991234567', 'contacto@innovatech.com', 
    'https://innovatech.com', 'https://innovatech.com/logo.png', 
    '2018-09-15', 'Activo', 'Empresa enfocada en soluciones fintech'
);

SELECT * FROM empleado;

INSERT INTO empleado (empresa_id, primer_nombre, segundo_nombre, apellido_paterno, apellido_materno, numero_identificacion, email_personal, email_corporativo, telefono, direccion, fecha_nacimiento, genero, estado, fecha_registro)
VALUES 
(1, 'ALEX', 'RUBEN', 'BARRE', 'GABILANES', '0953130390', 'alexruben611@gmail.com', 'maria.veloz@technologicalconsulting.com', '0967532016', 'COOP ENNER PARRALES MZ837 SL25A', '1999-11-06', 'Masculino', 'Activo', CURRENT_TIMESTAMP),
(1, 'MARIA', 'ISABEL', 'VELOZ', 'SUAREZ', '0954721536', 'mariaisabelvelozsuarez@gmail.com', 'maria.veloz@technologicalconsulting.com', '0980748006', 'Guasmo Sur Coop. Martha Bucarám de Roldós, Calles: 3 Pasaje 11H se y 6 Pa. 56 SE, Mz: 2327 Sl:10', '2000-10-10', 'Femenino', 'Activo', CURRENT_TIMESTAMP);


