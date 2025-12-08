import app from './app';
import config from './config/config';

const server = app.listen(config.port, function() {
  const address = server.address();
  const actualPort = typeof address === 'string' ? address : address?.port || config.port;
  console.log(`Server running on http://localhost:${actualPort}`);
  console.log(`Swagger docs available at http://localhost:${actualPort}/swagger`);
});