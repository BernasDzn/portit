export abstract class BaseMapper<Domain, Dto> {

	abstract toDto(domain: Domain): Dto;

	abstract toPersistence(domain: Domain): any;

	abstract fromSchema(schema: any): Domain;
	
}