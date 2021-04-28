import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { QueryBuilderClassNames, QueryBuilderConfig } from 'angular2-query-builder';

@Component({
  selector: 'app-filter-test',
  templateUrl: './filter-test.component.html',
  styleUrls: ['./filter-test.component.css']
})
export class FilterTestComponent implements OnInit {
  public queryCtrl: FormControl;


  public query = {
    condition: 'and',
    rules: [
      { field: 'age', operator: '<=', entity: 'physical' },
      { field: 'birthday', operator: '=', value: new Date(), entity: 'nonphysical' },
      {
        condition: 'or',
        rules: [
          { field: 'gender', operator: '=', entity: 'physical' },
          { field: 'occupation', operator: 'in', entity: 'nonphysical' },
          { field: 'school', operator: 'is null', entity: 'nonphysical' },
          { field: 'notes', operator: '=', entity: 'nonphysical' }
        ]
      }
    ]
  };

  public entityConfig: QueryBuilderConfig = {
    entities: {
      physical: { name: 'Physical Attributes' },
      nonphysical: { name: 'Nonphysical Attributes' }
    },
    fields: {
      age: { name: 'Age', type: 'number', entity: 'physical' },
      gender: {
        name: 'Gender',
        entity: 'physical',
        type: 'category',
        options: [
          { name: 'Male', value: 'NAM' },
          { name: 'Female', value: 'NU' }
        ]
      },
      name: { name: 'Name', type: 'string', entity: 'nonphysical' },
      notes: { name: 'Notes', type: 'textarea', operators: ['=', '!='], entity: 'nonphysical' },
      educated: { name: 'College Degree?', type: 'boolean', entity: 'nonphysical' },
      birthday: {
        name: 'Birthday', type: 'date', operators: ['=', '<=', '>'],
        defaultValue: (() => new Date()), entity: 'nonphysical'
      },
      school: { name: 'School', type: 'string', nullable: true, entity: 'nonphysical' },
      occupation: {
        name: 'Occupation',
        entity: 'nonphysical',
        type: 'category',
        options: [
          { name: 'Student', value: 'student' },
          { name: 'Teacher', value: 'teacher' },
          { name: 'Unemployed', value: 'unemployed' },
          { name: 'Scientist', value: 'scientist' }
        ]
      }
    }
  };

  public config: QueryBuilderConfig = {
    fields: {
      age: { name: 'Age', type: 'number' },
      gender: {
        name: 'Gender',
        type: 'category',
        options: [
          { name: 'Male', value: 'NAM' },
          { name: 'Female', value: 'NU' }
        ]
      },
      name: { name: 'Name', type: 'string' },
      notes: { name: 'Notes', type: 'textarea', operators: ['=', '!='] },
      educated: { name: 'College Degree?', type: 'boolean' },
      birthday: {
        name: 'Birthday', type: 'date', operators: ['=', '<=', '>'],
        defaultValue: (() => new Date())
      },
      school: { name: 'School', type: 'string', nullable: true },
      occupation: {
        name: 'Occupation',
        type: 'category',
        options: [
          { name: 'Student', value: 'student' },
          { name: 'Teacher', value: 'teacher' },
          { name: 'Unemployed', value: 'unemployed' },
          { name: 'Scientist', value: 'scientist' }
        ]
      }
    }
  };

  public currentConfig: QueryBuilderConfig;
  public allowRuleset: boolean = true;
  public allowCollapse: boolean;
  public persistValueOnFieldChange: boolean = false;

  constructor(
    private formBuilder: FormBuilder
  ) {
  }

  ngOnInit() {
    this.queryCtrl = this.formBuilder.control(this.query);
    this.currentConfig = this.config;
  }

  switchModes(event: Event) {
    this.currentConfig = (<HTMLInputElement>event.target).checked ? this.entityConfig : this.config;
  }

  changeDisabled(event: Event) {
    (<HTMLInputElement>event.target).checked ? this.queryCtrl.disable() : this.queryCtrl.enable();
  }
}
