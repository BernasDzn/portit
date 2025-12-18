import { Router } from "express";
import Container from "typedi";
import IncidentTypeController from "../../controllers/incidentTypeController";


const route = Router();

export default (app: Router) => {
	app.use('/incident-types', route);

	const getCtrl = () => Container.get(IncidentTypeController);

	route.get(
		'/count',
		(req, res, next) => getCtrl().count(req, res, next)
	);

	route.get(
		'/',
		(req, res, next) => getCtrl().getPaged(req, res, next)
	);

	route.get(
		'/:id',
		(req, res, next) => getCtrl().getById(req, res, next)
	);

	route.post(
		'/',
		(req, res, next) => getCtrl().createIncidentType(req, res, next)
	);


	route.put(
		'/:id',
		(req, res, next) => getCtrl().updateIncidentType(req, res, next)
	);
}