import { Injectable, EventEmitter, Output, Directive } from '@angular/core';
import { Subscription } from 'rxjs/internal/Subscription';    
@Directive()
@Injectable({
  providedIn: 'root'
})
export class EventEmitterService {
  @Output() invokeFirstComponentFunction = new EventEmitter();
  invokeUpdateMathPathFunction = new EventEmitter();
  invokeUpdateIsToggleFunction = new EventEmitter();
  subsVar: Subscription;
  subsVar2: Subscription;
  constructor() { }

  updateLeftMenuClick() {
    this.invokeFirstComponentFunction.emit();
  }
  updateMenuMapPath() {
    this.invokeUpdateMathPathFunction.emit();
  }
  updateIsToggleClick2() {
    this.invokeUpdateIsToggleFunction.emit();
  }
}
