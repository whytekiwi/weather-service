import {describe, it, expect, vi, beforeEach} from 'vitest';
import {render, screen, fireEvent} from '@testing-library/react';
import WeatherForm from './weatherForm';
import React from "react";
import MockUserProvider from "../../context/MockUserProvider.tsx";
import type {User} from "../../context/UserContext.tsx";
import {FetchWeather} from "../../services/weatherService.ts";
import type {Weather} from '../../services/weatherModel.ts';

vi.mock('../../services/weatherService');
const mockedWeatherService = vi.mocked(FetchWeather);

describe('WeatherForm', () => {

    let mockUser: User | null;

    function createMockUser(user: User | null = {id: 'test-api-key', name: 'Test User'}) {
        mockUser = user;
    }

    function mockFetchWeather(weather: Weather | null) {
        return mockedWeatherService.mockResolvedValue(weather);
    }

    const renderWithProvider = (children: React.ReactNode) =>
        render(<MockUserProvider user={mockUser}>{children}</MockUserProvider>);


    beforeEach(() => {
        createMockUser();
        vi.clearAllMocks();
    });

    it('renders form fields and submit button', () => {
        renderWithProvider(<WeatherForm/>);
        expect(screen.getByLabelText(/city/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/country/i)).toBeInTheDocument();
        expect(screen.getByRole('button', {name: /submit/i})).toBeInTheDocument();
    });

    it('shows error if no user is selected', async () => {
        createMockUser(null)
        renderWithProvider(<WeatherForm/>);
        fireEvent.change(screen.getByLabelText(/city/i), {target: {value: 'Melbourne'}});
        fireEvent.change(screen.getByLabelText(/country/i), {target: {value: 'AU'}});
        fireEvent.click(screen.getByRole('button', {name: /submit/i}));
        expect(await screen.findByText(/please select a user/i)).toBeInTheDocument();
    });

    it('shows error if no weather data is returned', async () => {
        mockFetchWeather(null);
        renderWithProvider(<WeatherForm/>);
        fireEvent.change(screen.getByLabelText(/city/i), {target: {value: 'Melbourne'}});
        fireEvent.change(screen.getByLabelText(/country/i), {target: {value: 'AU'}});
        fireEvent.click(screen.getByRole('button', {name: /submit/i}));
        expect(await screen.findByText(/no weather data found/i)).toBeInTheDocument();
    });

    it('shows weather data on success', async () => {
        mockFetchWeather({
            city: 'Melbourne',
            country: 'AU',
            description: 'Sunny',
        });
        renderWithProvider(<WeatherForm/>);
        fireEvent.change(screen.getByLabelText(/city/i), {target: {value: 'Melbourne'}});
        fireEvent.change(screen.getByLabelText(/country/i), {target: {value: 'AU'}});
        fireEvent.click(screen.getByRole('button', {name: /submit/i}));
        expect(await screen.findByText(/weather for Melbourne, AU/i)).toBeInTheDocument();
        expect(screen.getByText(/sunny/i)).toBeInTheDocument();
    });

    it('shows error on fetch failure', async () => {
        mockedWeatherService.mockRejectedValue(new Error('Network error'));
        renderWithProvider(<WeatherForm/>);
        fireEvent.change(screen.getByLabelText(/city/i), {target: {value: 'Melbourne'}});
        fireEvent.change(screen.getByLabelText(/country/i), {target: {value: 'AU'}});
        fireEvent.click(screen.getByRole('button', {name: /submit/i}));
        expect(await screen.findByText(/network error/i)).toBeInTheDocument();
    });
});