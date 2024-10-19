import { Component, OnInit, input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-validation-message',

  template: `
    @if (entityForm().controls[fieldName()]?.invalid && entityForm().controls[fieldName()]?.dirty) {
      <div>
        @for (validation of validationMessages()[fieldName()]; track validation) {
          <div>
            @if (entityForm().controls[fieldName()]?.errors?.[validation.type]) {
              <small id="username-help" class="p-error">{{ validation.message }}.</small>
            }
          </div>
        }
      </div>
    }
  `,
  standalone: true,
})
export class ValidationMessageComponent {
  entityForm = input.required<FormGroup>();
  fieldName = input.required<string>();
  validationMessages = input<any>();
}
