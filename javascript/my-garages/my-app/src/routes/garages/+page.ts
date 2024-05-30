import type {PageLoad} from './$types';
import type { Garage } from '$lib/models/Garage';

export const load: PageLoad = ({params}) => {
    const garages: Garage[] = [
        {
            id: '1',
            designation: 'Garage 1',
        },
        {
            id: '2',
            designation: 'Garage 2',
        },
        {
            id: '3',
            designation: 'Garage 3',
        },
    ];

    return {
        post: {
            garages,
        },
    };
};