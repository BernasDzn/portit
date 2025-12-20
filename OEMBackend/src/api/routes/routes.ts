/* tslint:disable */
/* eslint-disable */
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import type { TsoaRoute } from '@tsoa/runtime';
import {  fetchMiddlewares, ExpressTemplateService } from '@tsoa/runtime';
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import { VesselVisitExecutionController } from './../../controllers/vesselVisitExecutionController';
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import { TaskCategoryController } from './../../controllers/taskCategoryController';
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import { SchedulingRequestController } from './../../controllers/schedulingRequestController';
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import { OperationPlanController } from './../../controllers/operationPlanController';
// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
import { IncidentTypeController } from './../../controllers/incidentTypeController';
import type { Request as ExRequest, Response as ExResponse, RequestHandler, Router } from 'express';



// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

const models: TsoaRoute.Models = {
    "TaskCategoryDto": {
        "dataType": "refObject",
        "properties": {
            "id": {"dataType":"union","subSchemas":[{"dataType":"string"},{"dataType":"undefined"}],"required":true},
            "name": {"dataType":"string","required":true},
            "category": {"dataType":"string","required":true},
            "description": {"dataType":"string","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "ResourceDto": {
        "dataType": "refObject",
        "properties": {
            "name": {"dataType":"string","required":true},
            "type": {"dataType":"string","required":true},
            "startTime": {"dataType":"string"},
            "endTime": {"dataType":"string"},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "OperationDto": {
        "dataType": "refObject",
        "properties": {
            "type": {"ref":"TaskCategoryDto","required":true},
            "startTime": {"dataType":"string","required":true},
            "endTime": {"dataType":"string","required":true},
            "resources": {"dataType":"array","array":{"dataType":"refObject","ref":"ResourceDto"},"required":true},
            "payload": {"dataType":"any"},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "OperationStatusDto": {
        "dataType": "refAlias",
        "type": {"dataType":"union","subSchemas":[{"dataType":"enum","enums":["Pending"]},{"dataType":"enum","enums":["InProgress"]},{"dataType":"enum","enums":["Completed"]},{"dataType":"enum","enums":["Failed"]}],"validators":{}},
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "OperationWithStatusDto": {
        "dataType": "refObject",
        "properties": {
            "id": {"dataType":"string","required":true},
            "operation": {"ref":"OperationDto","required":true},
            "status": {"ref":"OperationStatusDto","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "VesselVisitExecutionStatusDto": {
        "dataType": "refAlias",
        "type": {"dataType":"union","subSchemas":[{"dataType":"enum","enums":["Open"]},{"dataType":"enum","enums":["Closed"]}],"validators":{}},
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "VesselVisitExecutionDto": {
        "dataType": "refObject",
        "properties": {
            "id": {"dataType":"string","required":true},
            "code": {"dataType":"string","required":true},
            "relatedVVN": {"dataType":"string","required":true},
            "operationsExecuted": {"dataType":"array","array":{"dataType":"refObject","ref":"OperationWithStatusDto"},"required":true},
            "dateOpen": {"dataType":"datetime"},
            "dateClosed": {"dataType":"datetime"},
            "status": {"ref":"VesselVisitExecutionStatusDto","required":true},
            "createdBy": {"dataType":"string","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "Page_VesselVisitExecutionDto_": {
        "dataType": "refObject",
        "properties": {
            "pageNumber": {"dataType":"double","required":true},
            "pageSize": {"dataType":"double","required":true},
            "pageCount": {"dataType":"double","required":true},
            "items": {"dataType":"array","array":{"dataType":"refObject","ref":"VesselVisitExecutionDto"},"required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "CreateTaskCategoryDto": {
        "dataType": "refObject",
        "properties": {
            "name": {"dataType":"string","required":true},
            "category": {"dataType":"string","required":true},
            "description": {"dataType":"string","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "Page_TaskCategoryDto_": {
        "dataType": "refObject",
        "properties": {
            "pageNumber": {"dataType":"double","required":true},
            "pageSize": {"dataType":"double","required":true},
            "pageCount": {"dataType":"double","required":true},
            "items": {"dataType":"array","array":{"dataType":"refObject","ref":"TaskCategoryDto"},"required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "WorkQueueItem": {
        "dataType": "refObject",
        "properties": {
            "day": {"dataType":"string","required":true},
            "alg": {"dataType":"string","required":true},
            "daysAhead": {"dataType":"double","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "ScheduleQueueItem": {
        "dataType": "refObject",
        "properties": {
            "id": {"dataType":"string","required":true},
            "data": {"ref":"WorkQueueItem","required":true},
            "priority": {"dataType":"double","required":true},
            "result": {"dataType":"any"},
            "requestedAt": {"dataType":"datetime","required":true},
            "status": {"dataType":"string","required":true},
            "issuer": {"dataType":"string","required":true},
            "estimatedStartTime": {"dataType":"union","subSchemas":[{"dataType":"datetime"},{"dataType":"enum","enums":[null]}],"required":true},
            "estimatedEndTime": {"dataType":"union","subSchemas":[{"dataType":"datetime"},{"dataType":"enum","enums":[null]}],"required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "OperationPlanMetadataDto": {
        "dataType": "refObject",
        "properties": {
            "createdBy": {"dataType":"string","required":true},
            "createdAt": {"dataType":"string","required":true},
            "algorithmUsed": {"dataType":"string","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "OperationPlanDto": {
        "dataType": "refObject",
        "properties": {
            "id": {"dataType":"string","required":true},
            "date": {"dataType":"string","required":true},
            "relatedVVN": {"dataType":"string","required":true},
            "dock": {"dataType":"string","required":true},
            "operationSchedule": {"dataType":"array","array":{"dataType":"refObject","ref":"OperationDto"},"required":true},
            "metadata": {"ref":"OperationPlanMetadataDto","required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "Page_OperationPlanDto_": {
        "dataType": "refObject",
        "properties": {
            "pageNumber": {"dataType":"double","required":true},
            "pageSize": {"dataType":"double","required":true},
            "pageCount": {"dataType":"double","required":true},
            "items": {"dataType":"array","array":{"dataType":"refObject","ref":"OperationPlanDto"},"required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "IncidentTypeDto": {
        "dataType": "refObject",
        "properties": {
            "bid": {"dataType":"string","required":true},
            "name": {"dataType":"string","required":true},
            "description": {"dataType":"string","required":true},
            "severity": {"dataType":"string","required":true},
            "subtypeOf": {"dataType":"string"},
            "subtypes": {"dataType":"array","array":{"dataType":"string"}},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "Page_IncidentTypeDto_": {
        "dataType": "refObject",
        "properties": {
            "pageNumber": {"dataType":"double","required":true},
            "pageSize": {"dataType":"double","required":true},
            "pageCount": {"dataType":"double","required":true},
            "items": {"dataType":"array","array":{"dataType":"refObject","ref":"IncidentTypeDto"},"required":true},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
    "PartialIncidentTypeDto": {
        "dataType": "refObject",
        "properties": {
            "name": {"dataType":"string","required":true},
            "description": {"dataType":"string","required":true},
            "severity": {"dataType":"string","required":true},
            "subtypeOf": {"dataType":"string"},
        },
        "additionalProperties": false,
    },
    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
};
const templateService = new ExpressTemplateService(models, {"noImplicitAdditionalProperties":"throw-on-extras","bodyCoercion":true});

// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa




export function RegisterRoutes(app: Router) {

    // ###########################################################################################################
    //  NOTE: If you do not see routes for all of your controllers in this file, then you might not have informed tsoa of where to look
    //      Please look into the "controllerPathGlobs" config option described in the readme: https://github.com/lukeautry/tsoa
    // ###########################################################################################################


    
        const argsVesselVisitExecutionController_openVesselVisitExecution: Record<string, TsoaRoute.ParameterSchema> = {
                relatedVVN: {"in":"path","name":"relatedVVN","required":true,"dataType":"string"},
        };
        app.post('/vessel-visit-executions/:relatedVVN/open',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.openVesselVisitExecution)),

            async function VesselVisitExecutionController_openVesselVisitExecution(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_openVesselVisitExecution, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'openVesselVisitExecution',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_closeVesselVisitExecution: Record<string, TsoaRoute.ParameterSchema> = {
                relatedVVN: {"in":"path","name":"relatedVVN","required":true,"dataType":"string"},
        };
        app.put('/vessel-visit-executions/:relatedVVN/close',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.closeVesselVisitExecution)),

            async function VesselVisitExecutionController_closeVesselVisitExecution(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_closeVesselVisitExecution, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'closeVesselVisitExecution',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_startOperation: Record<string, TsoaRoute.ParameterSchema> = {
                relatedVVN: {"in":"path","name":"relatedVVN","required":true,"dataType":"string"},
                operation: {"in":"body","name":"operation","required":true,"dataType":"any"},
        };
        app.put('/vessel-visit-executions/:relatedVVN/operations/start',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.startOperation)),

            async function VesselVisitExecutionController_startOperation(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_startOperation, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'startOperation',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_completeOperation: Record<string, TsoaRoute.ParameterSchema> = {
                relatedVVN: {"in":"path","name":"relatedVVN","required":true,"dataType":"string"},
                operationId: {"in":"path","name":"operationId","required":true,"dataType":"string"},
                body: {"in":"body","name":"body","required":true,"dataType":"nestedObjectLiteral","nestedProperties":{"endTime":{"dataType":"string","required":true}}},
        };
        app.put('/vessel-visit-executions/:relatedVVN/operations/:operationId/complete',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.completeOperation)),

            async function VesselVisitExecutionController_completeOperation(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_completeOperation, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'completeOperation',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_getVesselVisitExecution: Record<string, TsoaRoute.ParameterSchema> = {
                relatedVVN: {"in":"path","name":"relatedVVN","required":true,"dataType":"string"},
        };
        app.get('/vessel-visit-executions/:relatedVVN',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.getVesselVisitExecution)),

            async function VesselVisitExecutionController_getVesselVisitExecution(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_getVesselVisitExecution, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'getVesselVisitExecution',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_countVesselVisitExecutions: Record<string, TsoaRoute.ParameterSchema> = {
        };
        app.get('/vessel-visit-executions/count',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.countVesselVisitExecutions)),

            async function VesselVisitExecutionController_countVesselVisitExecutions(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_countVesselVisitExecutions, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'countVesselVisitExecutions',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsVesselVisitExecutionController_getAllVesselVisitExecutions: Record<string, TsoaRoute.ParameterSchema> = {
                page: {"default":1,"in":"query","name":"page","dataType":"double"},
                limit: {"default":10,"in":"query","name":"limit","dataType":"double"},
        };
        app.get('/vessel-visit-executions',
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController)),
            ...(fetchMiddlewares<RequestHandler>(VesselVisitExecutionController.prototype.getAllVesselVisitExecutions)),

            async function VesselVisitExecutionController_getAllVesselVisitExecutions(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsVesselVisitExecutionController_getAllVesselVisitExecutions, request, response });

                const controller = new VesselVisitExecutionController();

              await templateService.apiHandler({
                methodName: 'getAllVesselVisitExecutions',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsTaskCategoryController_createCategory: Record<string, TsoaRoute.ParameterSchema> = {
                categoryDto: {"in":"body","name":"categoryDto","required":true,"ref":"CreateTaskCategoryDto"},
        };
        app.post('/task-categories',
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController)),
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController.prototype.createCategory)),

            async function TaskCategoryController_createCategory(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsTaskCategoryController_createCategory, request, response });

                const controller = new TaskCategoryController();

              await templateService.apiHandler({
                methodName: 'createCategory',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsTaskCategoryController_updateCategory: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"path","name":"id","required":true,"dataType":"string"},
                categoryDto: {"in":"body","name":"categoryDto","required":true,"ref":"CreateTaskCategoryDto"},
        };
        app.put('/task-categories/:id',
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController)),
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController.prototype.updateCategory)),

            async function TaskCategoryController_updateCategory(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsTaskCategoryController_updateCategory, request, response });

                const controller = new TaskCategoryController();

              await templateService.apiHandler({
                methodName: 'updateCategory',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsTaskCategoryController_getCategoryByCode: Record<string, TsoaRoute.ParameterSchema> = {
                code: {"in":"path","name":"code","required":true,"dataType":"string"},
        };
        app.get('/task-categories/code/:code',
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController)),
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController.prototype.getCategoryByCode)),

            async function TaskCategoryController_getCategoryByCode(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsTaskCategoryController_getCategoryByCode, request, response });

                const controller = new TaskCategoryController();

              await templateService.apiHandler({
                methodName: 'getCategoryByCode',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsTaskCategoryController_getAllCategories: Record<string, TsoaRoute.ParameterSchema> = {
                pageNumber: {"default":0,"in":"query","name":"pageNumber","dataType":"double"},
                pageSize: {"default":10,"in":"query","name":"pageSize","dataType":"double"},
                name: {"in":"query","name":"name","dataType":"string"},
        };
        app.get('/task-categories',
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController)),
            ...(fetchMiddlewares<RequestHandler>(TaskCategoryController.prototype.getAllCategories)),

            async function TaskCategoryController_getAllCategories(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsTaskCategoryController_getAllCategories, request, response });

                const controller = new TaskCategoryController();

              await templateService.apiHandler({
                methodName: 'getAllCategories',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsSchedulingRequestController_scheduleRequest: Record<string, TsoaRoute.ParameterSchema> = {
                day: {"in":"query","name":"day","required":true,"dataType":"string"},
                alg: {"in":"query","name":"alg","required":true,"dataType":"string"},
                daysAhead: {"default":2,"in":"query","name":"daysAhead","dataType":"double"},
        };
        app.get('/schedule/request',
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController)),
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController.prototype.scheduleRequest)),

            async function SchedulingRequestController_scheduleRequest(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsSchedulingRequestController_scheduleRequest, request, response });

                const controller = new SchedulingRequestController();

              await templateService.apiHandler({
                methodName: 'scheduleRequest',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsSchedulingRequestController_getQueueState: Record<string, TsoaRoute.ParameterSchema> = {
        };
        app.get('/schedule/queueState',
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController)),
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController.prototype.getQueueState)),

            async function SchedulingRequestController_getQueueState(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsSchedulingRequestController_getQueueState, request, response });

                const controller = new SchedulingRequestController();

              await templateService.apiHandler({
                methodName: 'getQueueState',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsSchedulingRequestController_acceptRequest: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"query","name":"id","required":true,"dataType":"string"},
                request: {"in":"request","name":"request","required":true,"dataType":"object"},
        };
        app.post('/schedule/acceptRequest',
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController)),
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController.prototype.acceptRequest)),

            async function SchedulingRequestController_acceptRequest(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsSchedulingRequestController_acceptRequest, request, response });

                const controller = new SchedulingRequestController();

              await templateService.apiHandler({
                methodName: 'acceptRequest',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsSchedulingRequestController_rejectRequest: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"query","name":"id","required":true,"dataType":"string"},
        };
        app.post('/schedule/rejectRequest',
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController)),
            ...(fetchMiddlewares<RequestHandler>(SchedulingRequestController.prototype.rejectRequest)),

            async function SchedulingRequestController_rejectRequest(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsSchedulingRequestController_rejectRequest, request, response });

                const controller = new SchedulingRequestController();

              await templateService.apiHandler({
                methodName: 'rejectRequest',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_getPlans: Record<string, TsoaRoute.ParameterSchema> = {
                pageNumber: {"default":1,"in":"query","name":"pageNumber","dataType":"double"},
                pageSize: {"default":10,"in":"query","name":"pageSize","dataType":"double"},
                startDate: {"in":"query","name":"startDate","dataType":"string"},
                endDate: {"in":"query","name":"endDate","dataType":"string"},
        };
        app.get('/operation-plans',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.getPlans)),

            async function OperationPlanController_getPlans(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_getPlans, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'getPlans',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_getPlansByDate: Record<string, TsoaRoute.ParameterSchema> = {
        };
        app.get('/operation-plans/by-date',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.getPlansByDate)),

            async function OperationPlanController_getPlansByDate(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_getPlansByDate, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'getPlansByDate',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_getNotificationWithoutPlan: Record<string, TsoaRoute.ParameterSchema> = {
        };
        app.get('/operation-plans/notifications-without-plan',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.getNotificationWithoutPlan)),

            async function OperationPlanController_getNotificationWithoutPlan(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_getNotificationWithoutPlan, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'getNotificationWithoutPlan',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_regeneratePlansForDay: Record<string, TsoaRoute.ParameterSchema> = {
                body: {"in":"body","name":"body","required":true,"dataType":"nestedObjectLiteral","nestedProperties":{"daysAhead":{"dataType":"double"},"algorithm":{"dataType":"string","required":true},"day":{"dataType":"string","required":true}}},
        };
        app.post('/operation-plans/regenerate',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.regeneratePlansForDay)),

            async function OperationPlanController_regeneratePlansForDay(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_regeneratePlansForDay, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'regeneratePlansForDay',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_updateOperationPlan: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"path","name":"id","required":true,"dataType":"string"},
                planData: {"in":"body","name":"planData","required":true,"dataType":"any"},
        };
        app.patch('/operation-plans/:id',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.updateOperationPlan)),

            async function OperationPlanController_updateOperationPlan(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_updateOperationPlan, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'updateOperationPlan',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsOperationPlanController_getPlanById: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"path","name":"id","required":true,"dataType":"string"},
        };
        app.get('/operation-plans/:id',
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController)),
            ...(fetchMiddlewares<RequestHandler>(OperationPlanController.prototype.getPlanById)),

            async function OperationPlanController_getPlanById(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsOperationPlanController_getPlanById, request, response });

                const controller = new OperationPlanController();

              await templateService.apiHandler({
                methodName: 'getPlanById',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsIncidentTypeController_count: Record<string, TsoaRoute.ParameterSchema> = {
        };
        app.get('/incident-types/count',
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController)),
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController.prototype.count)),

            async function IncidentTypeController_count(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsIncidentTypeController_count, request, response });

                const controller = new IncidentTypeController();

              await templateService.apiHandler({
                methodName: 'count',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsIncidentTypeController_getPaged: Record<string, TsoaRoute.ParameterSchema> = {
                pageNumber: {"default":1,"in":"query","name":"pageNumber","dataType":"double"},
                pageSize: {"default":10,"in":"query","name":"pageSize","dataType":"double"},
        };
        app.get('/incident-types',
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController)),
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController.prototype.getPaged)),

            async function IncidentTypeController_getPaged(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsIncidentTypeController_getPaged, request, response });

                const controller = new IncidentTypeController();

              await templateService.apiHandler({
                methodName: 'getPaged',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsIncidentTypeController_getById: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"path","name":"id","required":true,"dataType":"string"},
        };
        app.get('/incident-types/:id',
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController)),
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController.prototype.getById)),

            async function IncidentTypeController_getById(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsIncidentTypeController_getById, request, response });

                const controller = new IncidentTypeController();

              await templateService.apiHandler({
                methodName: 'getById',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsIncidentTypeController_createIncidentType: Record<string, TsoaRoute.ParameterSchema> = {
                body: {"in":"body","name":"body","required":true,"ref":"PartialIncidentTypeDto"},
        };
        app.post('/incident-types',
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController)),
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController.prototype.createIncidentType)),

            async function IncidentTypeController_createIncidentType(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsIncidentTypeController_createIncidentType, request, response });

                const controller = new IncidentTypeController();

              await templateService.apiHandler({
                methodName: 'createIncidentType',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
        const argsIncidentTypeController_updateIncidentType: Record<string, TsoaRoute.ParameterSchema> = {
                id: {"in":"path","name":"id","required":true,"dataType":"string"},
                body: {"in":"body","name":"body","required":true,"ref":"PartialIncidentTypeDto"},
        };
        app.put('/incident-types/:id',
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController)),
            ...(fetchMiddlewares<RequestHandler>(IncidentTypeController.prototype.updateIncidentType)),

            async function IncidentTypeController_updateIncidentType(request: ExRequest, response: ExResponse, next: any) {

            // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

            let validatedArgs: any[] = [];
            try {
                validatedArgs = templateService.getValidatedArgs({ args: argsIncidentTypeController_updateIncidentType, request, response });

                const controller = new IncidentTypeController();

              await templateService.apiHandler({
                methodName: 'updateIncidentType',
                controller,
                response,
                next,
                validatedArgs,
                successStatus: undefined,
              });
            } catch (err) {
                return next(err);
            }
        });
        // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa

    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa


    // WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
}

// WARNING: This file was auto-generated with tsoa. Please do not modify it. Re-run tsoa to re-generate this file: https://github.com/lukeautry/tsoa
