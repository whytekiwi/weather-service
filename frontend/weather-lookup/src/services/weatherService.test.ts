import {FetchWeather} from './weatherService';
import type {User} from '../context/UserContext';
import type {Weather} from './weatherModel';

import {vi, describe, it, expect, beforeEach, type Mock} from 'vitest';

global.fetch = vi.fn();

const mockUser: User = {id: 'test-api-key', name: 'Test User'} as User;
const mockWeather: Weather = {description: 'Sunny'} as Weather;

describe('FetchWeather', () => {
    beforeEach(() => {
        (fetch as Mock).mockClear();
    });

    it('returns weather data on success', async () => {
        (fetch as Mock).mockResolvedValueOnce({
            ok: true,
            json: async () => ({...mockWeather}),
        });

        const result = await FetchWeather('Melbourne', 'AU', mockUser);
        expect(result).not.toBeNull();
        expect(result!.city).toBe('Melbourne');
        expect(result!.country).toBe('AU');
        expect(fetch).toHaveBeenCalled();
    });

    it('throws error when not successful', async () => {
        (fetch as Mock).mockResolvedValueOnce({ok: false, status: 401});
        await expect(FetchWeather('Melbourne', 'AU', mockUser)).rejects.toThrowError();
    });
});