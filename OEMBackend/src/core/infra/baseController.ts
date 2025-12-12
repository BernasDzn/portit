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
    public created (res: express.Response) {
        return res.sendStatus(201);
    }

    /**
     * returns a 400 bad request response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public clientError (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 400, message ? message : 'Bad request');
    }

    /**
     * returns a 401 unauthorized response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public unauthorized (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 401, message ? message : 'Unauthorized');
    }

    /**
     * returns a 402 payment required response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public paymentRequired (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 402, message ? message : 'Payment required');
    }

    /**
     * returns a 403 forbidden response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public forbidden (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 403, message ? message : 'Forbidden');
    }

    /**
     * returns a 404 not found response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public notFound (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 404, message ? message : 'Not found');
    }

    /**
     * returns a 409 conflict response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public conflict (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 409, message ? message : 'Conflict');
    }

    /**
     * returns a 429 too many requests response
     * @param res response object
     * @param message optional message
     * @returns {express.Response}
     */
    public tooMany (res: express.Response, message?: string) {
        return BaseController.jsonResponse(res, 429, message ? message : 'Too many requests');
    }

    /**
     * returns a 400 bad request response with TODO message
     * @param res response object
     * @returns {express.Response}
     */
    public todo (res: express.Response) {
        return BaseController.jsonResponse(res, 400, 'TODO');
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

}