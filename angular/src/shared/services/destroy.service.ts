import { OnDestroy } from '@angular/core';
import { Observable, pipe, Subject, takeUntil, UnaryFunction } from 'rxjs';

export class DestroyService<T> implements OnDestroy {
  readonly destroy$ = new Subject<void>();
  pipe<T>() {
    return pipe(takeUntil<T>(this.destroy$));
  }
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
