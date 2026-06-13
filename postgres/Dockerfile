FROM postgres:16-alpine

# Copy initialization scripts
COPY init/ /docker-entrypoint-initdb.d/

# Optimize for the app's workload
RUN echo "shared_buffers = 256MB" >> /usr/local/share/postgresql/postgresql.conf.sample && \
    echo "random_page_cost = 1.1" >> /usr/local/share/postgresql/postgresql.conf.sample && \
    echo "effective_cache_size = 768MB" >> /usr/local/share/postgresql/postgresql.conf.sample

HEALTHCHECK --interval=10s --timeout=5s --retries=5 \
    CMD pg_isready -U "${POSTGRES_USER:-ticketalert}" -d "${POSTGRES_DB:-ticketalert}"
