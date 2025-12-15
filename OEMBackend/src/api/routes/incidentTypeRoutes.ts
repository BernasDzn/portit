import { Router } from "express";
import Container from "typedi";
import IncidentTypeController from "../../controllers/incidentTypeController";


const route = Router();

export default (app: Router) => {
	app.use('/incident-types', route);

	const getCtrl = () => Container.get(IncidentTypeController);

	route.post(
		'/',
		(req, res, next) => getCtrl().createIncidentType(req, res, next)
	);

	route.get(
		'/',
		(req, res, next) => getCtrl().getAllIncidentTypes(req, res, next)
	);

	route.get(
		'/count',
		(req, res, next) => getCtrl().count(req, res, next)
	);

	route.get(
		'/:id',
		(req, res, next) => getCtrl().getIncidentTypeById(req, res, next)
	);

	route.patch(
		'/:id',
		(req, res, next) => getCtrl().updateIncidentType(req, res, next)
	);

	route.delete(
		'/:id/subtypes/:subtypeId',
		(req, res, next) => getCtrl().removeSubtype(req, res, next)
	);

	route.delete(
		'/:id',
		(req, res, next) => getCtrl().deleteIncidentType(req, res, next)
	);

}