
class Course {
    constructor(universityId, type, number, name, credits, description, components, notes, duration, prerequisites) {
        this.universityId = universityId;
        this.type = type;
        this.number = number;
        this.name = name;
        this.credits = credits;
        this.description = description;
        this.components = components;
        this.notes = notes;
        this.duration = duration;
        this.prerequisites = prerequisites;
    }
    
    toString(){
        return `${this.type.toUpperCase()} ${this.number}\n${this.name} (${this.credits} credits)`;
    }
}

function formatCourses(courses) {
    const container = $('#info-container');
    container.empty();

    courses.forEach(function(course) {
        const asCourse = new Course(course.universityId, course.type, course.number, course.name, course.credits, 
            course.description, course.components, course.notes, course.duration, course.prerequisites);
        const link = $('<a>')
            .attr('href', '/CourseViewer/CourseProfile?courseUniversityId=' + asCourse.universityId + '&courseType=' + asCourse.type + '&courseNumber=' + asCourse.number)
            .text(asCourse.toString());

        const tr = $('<tr>')
            .append(link)
            .append('<br>');
        container.append(tr);
    });
}

