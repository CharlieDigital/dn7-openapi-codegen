/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Address } from './Address';

/**
 * Represents a user profile.
 */
export type Profile = {
    /**
     * The ID of the profile.
     */
    id: string;
    /**
     * The name of the user.
     */
    name: string;
    /**
     * The email address of the user.
     */
    emailAddress: string;
    /**
     * The phone number of the user.
     */
    phoneNumber: string;
    address: Address;
};

