-- Crear la base de datos (si no existe)
CREATE DATABASE SGE ENCODING 'UTF8' LC_COLLATE='es_EC.UTF-8' LC_CTYPE='es_EC.UTF-8';

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

-- Tabla contactos_clientes
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
    contactos_clientes_id INTEGER REFERENCES contactos_clientes(id) ON DELETE CASCADE,
    fecha_asignacion DATE DEFAULT CURRENT_DATE,
    fecha_fin DATE NULL,
    estado VARCHAR(20) CHECK (estado IN ('Activo', 'Finalizado', 'Pendiente')) DEFAULT 'Activo',
    UNIQUE(empleado_id, cliente_id, contactos_clientes_id)
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
    presupuesto_salarial NUMERIC(10,2),
    nivel VARCHAR(20) NOT NULL CHECK (nivel IN ('training', 'junior', 'semisenior', 'senior')),
    UNIQUE(nombre, empresa_id)  
);

-- Tabla de User del Sistema (Autenticación)
CREATE TABLE users(
    id SERIAL PRIMARY KEY,
    empleado_id INTEGER UNIQUE REFERENCES empleado(id) ON DELETE CASCADE,
    usuario VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    intentos_fallidos INTEGER DEFAULT 0,
    fecha_ultimo_intento_fallido TIMESTAMP,
    bloqueado BOOLEAN DEFAULT FALSE,
    estado VARCHAR(20) CHECK (estado IN ('Activo', 'Inactivo', 'Bloqueado')) DEFAULT 'Activo',
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_ultimo_login TIMESTAMP DEFAULT NULL
);

CREATE TABLE historial_eventos_usuario (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    empleado_id INTEGER REFERENCES empleado(id) ON DELETE CASCADE,
    tipo_evento VARCHAR(50) NOT NULL CHECK (tipo_evento IN (
        'intento_acceso', 'bloqueo', 'recuperacion', 'sesion'
    )),
    fecha_evento TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    exito BOOLEAN,
    ip VARCHAR(45),
    navegador VARCHAR(255),
    razon TEXT,
    motivo TEXT,
    fecha_cambio TIMESTAMP
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
-- Evitar múltiples códigos activos del mismo tipo por usuario
    UNIQUE(usuario_id, tipo)
);




-- Información Laboral del Empelado
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
    notas TEXT -- Campo para observaciones adicionales
);