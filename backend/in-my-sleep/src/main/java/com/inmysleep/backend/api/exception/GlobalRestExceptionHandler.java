package com.inmysleep.backend.api.exception;

import com.inmysleep.backend.api.response.ApiResponse;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.MissingServletRequestParameterException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.http.converter.HttpMessageNotReadableException;

@RestControllerAdvice
public class GlobalRestExceptionHandler {

    @ExceptionHandler(NotFoundElementException.class)
    public ResponseEntity<ApiResponse<Void>> handleUserNotFoundException(NotFoundElementException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.NOT_FOUND);
    }

    @ExceptionHandler(DuplicateException.class)
    public ResponseEntity<ApiResponse<Void>> handleDuplicateException(DuplicateException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.CONFLICT);
    }

    @ExceptionHandler(InvalidDataException.class)
    public ResponseEntity<ApiResponse<Void>> handleInvalidDataException(InvalidDataException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<ApiResponse<Void>> handleMethodArgumentNotValidException(MethodArgumentNotValidException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, "Invalid input data: " + e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(MissingServletRequestParameterException.class)
    public ResponseEntity<ApiResponse<Void>> handleMissingServletRequestParameterException(MissingServletRequestParameterException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, "Missing required parameter: " + e.getParameterName());
        return new ResponseEntity<>(response, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(HttpMessageNotReadableException.class)
    public ResponseEntity<ApiResponse<Void>> handleHttpMessageNotReadableException(HttpMessageNotReadableException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, "Malformed JSON request: " + e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ApiResponse<Void>> handleGeneralException(Exception e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, "An unexpected error occurred : " + e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.INTERNAL_SERVER_ERROR);
    }

    @ExceptionHandler(EmailSendException.class)
    public ResponseEntity<ApiResponse<Void>> handleUnableToSendEmailException(EmailSendException e) {
        ApiResponse<Void> response = new ApiResponse<>();
        response.setResponseFalse(null, "Unable to send email: " + e.getMessage());
        return new ResponseEntity<>(response, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
