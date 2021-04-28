import { FormControl } from '@angular/forms';
import * as moment from 'moment';

export class ValidatorsCommon {

    // Validation date according format dd/MM/yyyy
    static formatDate(control: FormControl) {
        if (control && control.value && !moment(control.value, 'YYYY-MM-DD', true).isValid()) {
            return { 'formatDate': true };
        }
        return null;
    }

    // Validation object null when use mat-auto-complete
    static isInValidObject(control: FormControl) {
        const pattern = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
        if (control.value !== '') {
            if (pattern.test(control.value) === false) {
                return { 'isInValidObject': true };
            }
        }
        return null;
    }

}
