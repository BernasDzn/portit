import type { BaseMapper } from "../core/infra/baseMapper";
import { Page } from "../utils/page";

export class PageMapper {

	static itemsToDto<Domain, Dto>(
		page: Page<Domain>,
		mapper: BaseMapper<Domain, Dto>
	): Page<Dto> {
		return {
			items: page.items.map(item => mapper.toDto(item)),
			pageNumber: page.pageNumber,
			pageSize: page.pageSize,
			pageCount: page.pageCount
		};
	}
}