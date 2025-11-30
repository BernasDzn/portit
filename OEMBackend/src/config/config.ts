import dotenv from 'dotenv';

dotenv.config();

interface Config {
	port: number;
	nodeEnv: string;
    mongoUri: string;
    shouldBootstrap?: boolean;
    jwt: {
        jwtSecret: string;
        jwtIssuer: string;
        jwtAudience: string;
    };
}

const config: Config = {
	port: Number(process.env.PORT) || 3000,
	nodeEnv: process.env.NODE_ENV || 'development',
    mongoUri: process.env.MONGO_URI || '',
    jwt: {
        jwtSecret: process.env.JWT_SECRET || '',
        jwtIssuer: process.env.JWT_ISSUER || 'http://localhost:2226',
        jwtAudience: process.env.JWT_AUDIENCE || 'http://localhost:5173',
    },
    shouldBootstrap: process.env.SHOULD_BOOTSTRAP === 'true',
};

export default config;