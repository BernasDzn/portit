import * as express from 'express'

export abstract class BaseController {

    /**
     * returns a JSON response
     * @param res response object
     * @param code HTTP status code
     * @param message message
     * @returns {express.Response}
     */
    public static jsonResponse (res: express.Response, code: number, message: string) {
        return res.status(code).json({ message })
    }

    /**
     * returns a 200 OK response
     * @param res response object
     * @param dto optional data transfer object
     * @returns {express.Response}
     */
    public ok<T> (res: express.Response, dto?: T) {
        if (!!dto) {
            return res.status(200).json(dto);
        } else {
            return res.sendStatus(200);
        }
    }

    /**
     * returns a 201 Created response
     * @param res response object
     * @returns {express.Response}
     */
    public created (res: express.Response, dto?: any) {
        if (!!dto) {
            return res.status(201).json(dto);
        } else {
            return res.sendStatus(201);
        }
    }

    /**
     * returns a 400 bad request response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public clientError (res: express.Response, message?: string) {
        if(!!message){
            return res.status(400).json(message);
        }else{
            return res.sendStatus(400);
        }
    }

    /**
     * returns a 401 unauthorized response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public unauthorized (res: express.Response, message?: string) {
        if(!!message){
            return res.status(401).json(message);
        }else{
            return res.sendStatus(401);
        }
    }

    /**
     * returns a 402 payment required response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public paymentRequired (res: express.Response, message?: string) {
        if(!!message){
            return BaseController.jsonResponse(res, 402, message);
        }else{
            return res.sendStatus(402);
        }
    }

    /**
     * returns a 403 forbidden response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public forbidden (res: express.Response, message?: string) {
        if(!!message){
            return res.status(403).json(message);
        }else{
            return res.sendStatus(403);
        }
    }

    /**
     * returns a 404 not found response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public notFound (res: express.Response, message?: string) {
        if(!!message){
            return res.status(404).json(message);
        }else{
            return res.sendStatus(404);
        }
    }

    /**
     * returns a 409 conflict response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public conflict (res: express.Response, message?: string) {
        if(!!message){
            return res.status(409).json(message);
        }else{
            return res.sendStatus(409);
        }
    }

    /**
     * returns a 429 too many requests response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public tooMany (res: express.Response, message?: string) {
        if(!!message){
            return res.status(429).json(message);
        }else{
            return res.sendStatus(429);
        }
    }

    /**
     * returns a 500 internal server error response
     * @param res response object
     * @param error error message or object
     * @returns {express.Response}
     */
    public fail (res: express.Response, error: Error | string) {
        console.log(error);
        return res.status(500).json({
            message: error.toString()
        })
    }

    /**
     * returns a 400 bad request response with TODO message
     * @param res response object
     * @returns {express.Response}
     */
    public todo (res: express.Response) {
        return BaseController.jsonResponse(res, 400, 'TODO');
    }

}