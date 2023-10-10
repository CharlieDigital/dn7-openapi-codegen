/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Profile } from '../models/Profile';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class ProfileService {

    /**
     * Gets the profiles from the backend
     * @returns Profile Success
     * @throws ApiError
     */
    public static getProfiles(): CancelablePromise<Array<Profile>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/profiles',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }

}
