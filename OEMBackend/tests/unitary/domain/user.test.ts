import { User, UserRole } from "../../../src/domain/user";

describe('User', () => {
	describe('constructor', () => {
		it('should create a user with all required properties', () => {
			const user = new User({
				id: 'user-1',
				emailAddress: 'john.doe@example.com',
				name: 'John Doe',
				userRole: UserRole.Administrator
			});

			expect(user.id).toBe('user-1');
			expect(user.emailAddress).toBe('john.doe@example.com');
			expect(user.name).toBe('John Doe');
			expect(user.userRole).toBe(UserRole.Administrator);
		});

		it('should support different user roles', () => {
			const roles = [
				UserRole.Administrator,
				UserRole.PortAuthorityOfficer,
				UserRole.SAORepresentative,
				UserRole.LogisticsOperator
			];

			roles.forEach(role => {
				const user = new User({
					id: `user-${role}`,
					emailAddress: `user@${role}.com`,
					name: `User ${role}`,
					userRole: role
				});

				expect(user.userRole).toBe(role);
			});
		});
	});

	describe('toDto', () => {
		it('should convert user to DTO with correct property mapping', () => {
			const user = new User({
				id: 'user-123',
				emailAddress: 'jane.smith@example.com',
				name: 'Jane Smith',
				userRole: UserRole.LogisticsOperator
			});

			const dto = user.toDto();

			expect(dto.id).toBe('user-123');
			expect(dto.emailAddress).toBe('jane.smith@example.com');
			expect(dto.name).toBe('Jane Smith');
			expect(dto.user_role).toBe(UserRole.LogisticsOperator);
		});

		it('should preserve role enum in DTO', () => {
			const user = new User({
				id: 'user-456',
				emailAddress: 'officer@port.com',
				name: 'Port Officer',
				userRole: UserRole.PortAuthorityOfficer
			});

			const dto = user.toDto();

			expect(dto.user_role).toBe('PortAuthorityOfficer');
		});
	});
});
