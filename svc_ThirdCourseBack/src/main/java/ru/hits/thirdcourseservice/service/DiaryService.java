package ru.hits.thirdcourseservice.service;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import ru.hits.thirdcourseservice.dto.AddDiaryDto;
import ru.hits.thirdcourseservice.dto.AddDiaryWithStudentIdDto;
import ru.hits.thirdcourseservice.dto.DiaryDto;
import ru.hits.thirdcourseservice.dto.DiaryFeedbackDto;
import ru.hits.thirdcourseservice.entity.DiaryEntity;
import ru.hits.thirdcourseservice.entity.SemesterEntity;
import ru.hits.thirdcourseservice.entity.StudentInSemesterEntity;
import ru.hits.thirdcourseservice.exception.NotFoundException;
import ru.hits.thirdcourseservice.repository.DiaryRepository;
import ru.hits.thirdcourseservice.repository.SemesterRepository;
import ru.hits.thirdcourseservice.repository.StudentInSemesterRepository;

import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;

@Slf4j
@Service
@RequiredArgsConstructor
public class DiaryService {

    private final DiaryRepository diaryRepository;
    private final StudentInSemesterRepository studentInSemesterRepository;
    private final SemesterRepository semesterRepository;

    @Transactional
    public void addDiary(AddDiaryDto addDiaryDto) {
        StudentInSemesterEntity studentInSemester = studentInSemesterRepository.findById(addDiaryDto.getStudentInSemesterId())
                .orElseThrow(() -> new NotFoundException("Студент в семестре с ID " + addDiaryDto.getStudentInSemesterId() + " не найден"));

        DiaryEntity diary = DiaryEntity.builder()
                .documentId(addDiaryDto.getDocumentId())
                .attachedAt(LocalDateTime.now())
                .studentInSemesterEntity(studentInSemester)
                .build();

        diaryRepository.save(diary);
        studentInSemester.setDiary(diary);
        studentInSemesterRepository.save(studentInSemester);
    }

    @Transactional
    public void addDiaryToStudent(AddDiaryWithStudentIdDto addDiaryWithStudentIdDto) {
        StudentInSemesterEntity studentInSemester = studentInSemesterRepository.findByStudentId(addDiaryWithStudentIdDto.getStudentId())
                .orElseThrow(() -> new NotFoundException("Студент с ID " + addDiaryWithStudentIdDto.getStudentId() + " не найден"));

        DiaryEntity diary = DiaryEntity.builder()
                .documentId(addDiaryWithStudentIdDto.getDocumentId())
                .attachedAt(LocalDateTime.now())
                .studentInSemesterEntity(studentInSemester)
                .build();

        diaryRepository.save(diary);
        studentInSemester.setDiary(diary);
        studentInSemesterRepository.save(studentInSemester);
    }

    public DiaryDto getDiary(UUID diaryId) {
        DiaryEntity diary = diaryRepository.findById(diaryId)
                .orElseThrow(() -> new NotFoundException("Дневник с ID " + diaryId + " не найден"));

        return DiaryDto.builder()
                .id(diary.getId())
                .documentId(diary.getDocumentId())
                .attachedAt(diary.getAttachedAt())
                .diaryFeedback(diary.getDiaryFeedback() != null ? new DiaryFeedbackDto(diary.getDiaryFeedback()) : null)
                .build();
    }

    public DiaryDto getStudentDiary(UUID semesterId, UUID studentId) {
        SemesterEntity semester = semesterRepository.findById(semesterId)
                .orElseThrow(() -> new NotFoundException("Семестр с ID " + semesterId + " не найден"));

        StudentInSemesterEntity studentInSemester = studentInSemesterRepository.findByStudentIdAndSemester(studentId, semester)
                .orElseThrow(() -> new NotFoundException("Студент с ID " + studentId + " в семестре c ID " + semesterId + " не найден"));

        if (studentInSemester.getDiary() == null) {
            return null;
        } else {
            return DiaryDto.builder()
                    .id(studentInSemester.getDiary().getId())
                    .documentId(studentInSemester.getDiary().getDocumentId())
                    .attachedAt(studentInSemester.getDiary().getAttachedAt())
                    .diaryFeedback(studentInSemester.getDiary().getDiaryFeedback() != null ? new DiaryFeedbackDto(studentInSemester.getDiary().getDiaryFeedback()) : null)
                    .build();
        }
    }

}
