import { Injectable, OnDestroy } from '@angular/core';
import { pipe, Subject, takeUntil } from 'rxjs';

@Injectable()
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
