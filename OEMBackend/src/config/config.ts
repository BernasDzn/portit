import dotenv from 'dotenv';

dotenv.config();

interface Config {
	port: number;
	nodeEnv: string;
    mongoUri: string;
    jwtSecret: string;
    shouldBootstrap?: boolean;
}

const config: Config = {
	port: Number(process.env.PORT) || 3000,
	nodeEnv: process.env.NODE_ENV || 'development',
    mongoUri: process.env.MONGO_URI || '',
    jwtSecret: process.env.JWT_SECRET || '',
    shouldBootstrap: process.env.SHOULD_BOOTSTRAP === 'true',
};

export default config;