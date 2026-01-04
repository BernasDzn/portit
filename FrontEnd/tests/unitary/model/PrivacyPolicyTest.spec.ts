import { describe, it, expect } from 'vitest';
import { PrivacyPolicy } from '../../../src/model/PrivacyPolicy';

describe('PrivacyPolicy', () => {
    describe('Constructor', () => {
        it('should create privacy policy with all fields', () => {
            const updatedOn = new Date('2024-01-01');
            const privacyPolicy = new PrivacyPolicy({
                content: 'This is the privacy policy content',
                updatedOn,
                active: true
            });

            expect(privacyPolicy.content).toBe('This is the privacy policy content');
            expect(privacyPolicy.updatedOn).toEqual(updatedOn);
            expect(privacyPolicy.active).toBe(true);
        });

        it('should create inactive privacy policy', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Old policy',
                updatedOn: new Date('2023-01-01'),
                active: false
            });

            expect(privacyPolicy.active).toBe(false);
        });

        it('should throw error when content is empty', () => {
            expect(() => new PrivacyPolicy({
                content: '',
                updatedOn: new Date(),
                active: true
            })).toThrow('Content cannot be null or empty.');
        });

        it('should throw error when content is null', () => {
            expect(() => new PrivacyPolicy({
                content: null as any,
                updatedOn: new Date(),
                active: true
            })).toThrow('Content cannot be null or empty.');
        });
    });

    describe('isActive', () => {
        it('should return true when policy is active', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Content',
                updatedOn: new Date(),
                active: true
            });

            expect(privacyPolicy.isActive()).toBe(true);
        });

        it('should return false when policy is inactive', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Content',
                updatedOn: new Date(),
                active: false
            });

            expect(privacyPolicy.isActive()).toBe(false);
        });
    });

    describe('updateContent', () => {
        it('should update content successfully', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Old content',
                updatedOn: new Date(),
                active: true
            });

            privacyPolicy.updateContent('New content');

            expect(privacyPolicy.content).toBe('New content');
        });

        it('should throw error when updating with empty content', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Old content',
                updatedOn: new Date(),
                active: true
            });

            expect(() => privacyPolicy.updateContent('')).toThrow('Content cannot be null or empty.');
        });

        it('should throw error when updating with null content', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Old content',
                updatedOn: new Date(),
                active: true
            });

            expect(() => privacyPolicy.updateContent(null as any)).toThrow('Content cannot be null or empty.');
        });
    });

    describe('deactivate', () => {
        it('should deactivate active policy', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Content',
                updatedOn: new Date(),
                active: true
            });

            privacyPolicy.deactivate();

            expect(privacyPolicy.active).toBe(false);
            expect(privacyPolicy.isActive()).toBe(false);
        });

        it('should keep inactive policy inactive', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Content',
                updatedOn: new Date(),
                active: false
            });

            privacyPolicy.deactivate();

            expect(privacyPolicy.active).toBe(false);
        });
    });

    describe('toDto', () => {
        it('should convert to DTO correctly', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Privacy policy content',
                updatedOn: new Date('2024-01-01'),
                active: true
            });

            const dto = privacyPolicy.toDto();

            expect(dto.content).toBe('Privacy policy content');
        });

        it('should only include content in DTO', () => {
            const privacyPolicy = new PrivacyPolicy({
                content: 'Test content',
                updatedOn: new Date('2024-01-01'),
                active: true
            });

            const dto = privacyPolicy.toDto();

            expect(Object.keys(dto)).toEqual(['content']);
        });
    });
});
