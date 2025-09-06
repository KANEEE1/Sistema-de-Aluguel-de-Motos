# Como Executar o Sistema de Aluguel de Motos

## Pré-requisitos
- Docker e Docker Compose instalados

## Execução
```bash
# 1. Clone o repositório
git clone git@github.com:KANEEE1/Sistema-de-Aluguel-de-Motos.git

# 2. Navegue para o diretório
cd Sistema-de-Aluguel-de-Motos

# 3. Execute com Docker
docker compose up --build

# 4. Acesse a API
http://localhost:8080/swagger
```

## Autenticação

### Criar Usuários
```bash
# Criar usuário Admin
POST /auth/register
{
  "username": "admin",
  "email": "admin@test.com",
  "password": "admin123",
  "role": "Admin"
}

# Criar usuário Entregador
POST /auth/register
{
  "username": "entregador1",
  "email": "entregador@test.com", 
  "password": "entregador123",
  "role": "DeliveryPerson"
}
```

### Login
```bash
POST /auth/login
{
  "username": "admin",
  "password": "admin123"
}
```

### Usar nas Requisições
Adicione o header: `X-Username: admin` (ou o username do usuário logado)

## Endpoints Principais

### Motorcycles (Admin apenas para criar/editar/deletar)
- `POST /motorcycles` - Criar moto (Admin)
- `GET /motorcycles` - Listar motos (todos)
- `GET /motorcycles/{id}` - Buscar moto por ID (todos)
- `PUT /motorcycles/{id}/plate` - Atualizar placa (Admin)
- `DELETE /motorcycles/{id}` - Deletar moto (Admin)

**Exemplo de request para criar moto:**
```json
{
  "identificador": "moto123",
  "ano": 2024,
  "modelo": "m1",
  "placa": "ABC1234"
}
```

**Exemplo de request para atualizar placa:**
```json
{
  "placa": "XYZ9876"
}
```

### Delivery People
- `POST /delivery-people` - Criar entregador (todos)
- `GET /delivery-people` - Listar entregadores (todos)
- `GET /delivery-people/{id}` - Buscar entregador (todos)
- `PUT /delivery-people/{id}/cnh` - Atualizar imagem CNH (todos)

**Exemplo de request para criar entregador:**
```json
{
  "identificador": "entregador123",
  "nome": "Felipe",
  "cnpj": "12345678901234",
  "data_nascimento": "1990-01-01T00:00:00Z",
  "numero_cnh": "12345678900",
  "tipo_cnh": "A",
  "imagem_cnh": "base64string"
}
```

**Exemplo de request para atualizar imagem CNH:**
```json
{
  "imagem_cnh": "base64string"
}
```

### Rentals
- `POST /rentals` - Criar locação (todos)
- `GET /rentals` - Listar locações (todos)
- `GET /rentals/{id}` - Buscar locação (todos)
- `PUT /rentals/{id}/return` - Informar data de devolução (todos)
- `GET /rentals/{id}/total-value` - Consultar valor total da locação (todos)

**Exemplo de request para criar locação:**
```json
{
  "idetificador_entregador": "entregador123",
  "identificador_moto": "moto123",
  "data_inicio": "2024-01-01",
  "data_termino": "2024-01-07",
  "data_previsao_termino": "2024-01-07",
  "plano": 7
}
```

**Exemplo de request para devolução:**
```json
{
  "data_devolucao": "2024-01-07"
}
```
