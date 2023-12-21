import { Injectable, OnDestroy } from '@angular/core';
import { pipe, Subject, takeUntil } from 'rxjs';

@Injectable()
export class DestroyService<T> implements OnDestroy {
  readonly destroy$ = new Subject<void>();
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
