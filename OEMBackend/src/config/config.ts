import dotenv from 'dotenv';
import raw from '../../config.json';

dotenv.config();

interface Config {
	port: number;
	nodeEnv: string;
    mongoUri: string;
    shouldBootstrap?: boolean;
    disableAuth?: boolean;
    jwt: {
        jwtSecret: string;
        jwtIssuer: string;
        jwtAudience: string;
    };
    schedulingServer: string;
    backendServer: string;
}

const runMode = (process.env.RUN_MODE || 'dev') as 'dev' | 'local';
const modeConfig = raw[runMode];

console.log(`Running in ${runMode} mode`);

const config: Config = {
	port: Number(process.env.PORT) || raw.port,
	nodeEnv: process.env.NODE_ENV || 'development',
    mongoUri: process.env.MONGO_URI || raw.mongoUri,
    jwt: {
        jwtSecret: process.env.JWT_SECRET || raw.jwtSecret,
        jwtIssuer: process.env.JWT_ISSUER || modeConfig.jwtIssuer,
        jwtAudience: process.env.JWT_AUDIENCE || modeConfig.jwtAudience,
    },
    shouldBootstrap: process.env.SHOULD_BOOTSTRAP === 'true',
    disableAuth: process.env.DISABLE_AUTH === 'true',
    schedulingServer: process.env.SCHEDULING_SERVER || modeConfig.schedulingServer,
    backendServer: process.env.BACKEND_SERVER || modeConfig.backendServer,
};

export default config;