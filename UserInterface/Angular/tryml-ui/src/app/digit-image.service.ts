import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DigitImageService {

  constructor(private httpClient: HttpClient) { 
  }

  /**
   * @name getDigitImage
   * @desc Retrieves the pixels data of a digit image from the remote digit image service.
   *
   * @param {number} imageId - The numeric identifier of the image to retrieve.
   * @returns {Observable<number[][]>} An observable which publishes a 2d array containing byte values (0-255) of the pixels in the digit image.
   */
  public getDigitImage(imageId: number): Observable<number[][]> {

    if (imageId < 0 || imageId > 2)
      throw new Error("Parameter 'imageId' must be between 0 and 2 inclusive.");

    let digitImageUri: string = "http://localhost:9000/api/DigitImages/" + imageId.toString();
    return this.httpClient.get<number[][]>(digitImageUri);
  }
}
