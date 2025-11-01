// This mapper normalizes staff data payloads from the backend
// Idk if we should use this pattern, might need to ask the teacher
export const StaffMapper = {
	normalizeStaffPayload(payload: any[]): { 
		mecanograficNumber: string; 
		name: string
	}[] {
		return payload.map(s => ({
			mecanograficNumber: s.mechanograficNumber ?? '',
			name: s.name ?? '',
		}))
	}
}