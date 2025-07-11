import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import App from './App';

describe('App Component', () => {
    it('renders hello world', () => {
        render(<App />);
        expect(screen.getByText('Weather Look-up')).toBeInTheDocument();
    });
});