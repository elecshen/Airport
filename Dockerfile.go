# Go 1.22 - Flight Service
FROM golang:1.22-alpine AS build

WORKDIR /app

# Install dependencies
RUN apk add --no-cache git ca-certificates

# Copy go mod files
COPY services/flights/go.mod services/flights/go.sum ./
RUN go mod download

# Copy source code
COPY services/flights/. ./

# Build with optimizations for smaller binary
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -ldflags="-w -s" -o /app/flight-service .

# Runtime image
FROM alpine:3.19

RUN apk --no-cache add ca-certificates tzdata

WORKDIR /app
COPY --from=build /app/flight-service .

EXPOSE 8080

ENTRYPOINT ["./flight-service"]
