#Dockerfile

# Use this image as the platform to build the app

FROM node:23.3 AS external-website

# A small line inside the image to show who made it
# LABEL Developers="Loic Joachim & Cameron Hill"

# The WORKDIR instruction sets the working directory for everything that will happen next
WORKDIR /app

# Copy all local files into the image
COPY . .

# Clean install all node modules

ENV NODE_ENV=production
ENV VITE_BASE_URL=/password/reset/api

# Build SvelteKit app 
WORKDIR /app
RUN npm ci
RUN npm run build

# Delete source code files that were used to build the app that are no longer needed
# RUN rm -rf src/ static/ emailTemplates/ docker-compose.yml

# The USER instruction sets the user name to use as the default user for the remainder of the current stage
USER node:node

EXPOSE 3000

# This is the command that will be run inside the image when you tell Docker to start the container
CMD ["node","build/index.js"]
