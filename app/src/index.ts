/**
 * Sample file consuming the services and models
 */

import { ProfileService } from "./_api/index";

const profiles = await ProfileService.getProfiles()

console.log(profiles[0].name)

