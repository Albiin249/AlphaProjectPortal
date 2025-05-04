document.addEventListener('DOMContentLoaded', () => {
    updateRelativeTimes();
    setInterval(updateRelativeTimes, 60000);

    // Initiate all Quill-editors
    document.querySelectorAll('[data-quill-editor]').forEach(editorWrapper => {
        const editorId = editorWrapper.getAttribute('id');
        const toolbarId = editorWrapper.dataset.toolbar;
        const targetTextareaId = editorWrapper.dataset.target;

        const quill = new Quill(`#${editorId}`, {
            modules: {
                syntax: true,
                toolbar: `#${toolbarId}`
            },
            theme: 'snow',
            placeholder: 'Type something'
        });


        const targetTextarea = document.getElementById(targetTextareaId);
        quill.on('text-change', () => {
            targetTextarea.value = quill.root.innerHTML;
        });

        editorWrapper._quill = quill;

    });


    // Initiate fileuploads
    document.querySelectorAll('[data-upload-trigger]').forEach(trigger => {
        const inputId = trigger.dataset.uploadTrigger
        const fileInput = document.getElementById(inputId)

        const imagePreview = document.querySelector(`[data-preview="${inputId}"]`)
        const imagePreviewIcon = document.querySelector(`[data-icon="${inputId}"]`)
        const imagePreviewContainer = document.querySelector(`[data-container="${inputId}"]`)

        trigger.addEventListener('click', () => fileInput.click())

        fileInput.addEventListener('change', (e) => {
            const file = e.target.files[0]
            if (file && file.type.startsWith('image/')) {
                const reader = new FileReader()
                reader.onload = (e) => {
                    imagePreview.src = e.target.result
                    imagePreview.classList.remove('hide')
                    imagePreviewContainer?.classList.add('selected')
                    imagePreviewIcon?.classList.replace('fa-camera', 'fa-pen')
                }
                reader.readAsDataURL(file)
            }
        })
    })


    // Dropdowns
    document.querySelectorAll('[data-type="dropdown"]').forEach(dropdown => {
        dropdown.addEventListener('click', (e) => {
            e.stopPropagation()
            const targetSelector = dropdown.getAttribute('data-target')
            const targetElement = document.querySelector(targetSelector)

            document.querySelectorAll('.dropdown.dropdown-show').forEach(d => {
                if (d !== targetElement) d.classList.remove('dropdown-show')
            })

            targetElement?.classList.toggle('dropdown-show')
        })
    })

    document.addEventListener('click', e => {
        if (!e.target.closest('.dropdown')) {
            document.querySelectorAll('.dropdown.dropdown-show').forEach(el => el.classList.remove('dropdown-show'))
        }
    })


    // Modals
    document.querySelectorAll('[data-type="modal"]').forEach(modalTrigger => {
        modalTrigger.addEventListener('click', () => {
            const target = document.querySelector(modalTrigger.dataset.target)
            target?.classList.add('modal-show')
        })
    })

    document.querySelectorAll('[data-type="close-project-modal"]').forEach(closeBtn => {
        closeBtn.addEventListener('click', () => {
            const target = document.querySelector(closeBtn.dataset.target)
            target?.classList.remove('modal-show')
        })
    })


    //Edit project, member & client
    document.querySelectorAll('[data-action="edit-project"]').forEach(button => {
        button.addEventListener('click', () => {
            const id = button.dataset.projectId;
            const name = button.dataset.name;
            const description = button.dataset.description;
            const budget = button.dataset.budget;
            const clientId = button.dataset.clientId;
            const clientName = button.dataset.clientName;
            const statusId = button.dataset.statusId;
            const statusName = button.dataset.statusName;
            const startDate = button.dataset.startDate;
            const endDate = button.dataset.endDate;
            const image = button.dataset.image; 

            document.querySelector('#edit-project-modal input[name="Id"]').value = id;
            document.querySelector('#edit-project-modal input[name="ProjectName"]').value = name;
            document.querySelector('#edit-project-modal input[name="Budget"]').value = budget;
            document.querySelector('#edit-project-modal input[name="ClientId"]').value = clientId;
            document.querySelector('#edit-project-modal [data-target="client-select-text"]').textContent = clientName;
            document.querySelector('#edit-project-modal input[name="StatusId"]').value = statusId;
            document.querySelector('#edit-project-modal [data-target="status-select-text"]').textContent = statusName;
            document.querySelector('#edit-project-modal input[name="StartDate"]').value = startDate;
            document.querySelector('#edit-project-modal input[name="EndDate"]').value = endDate;
            document.querySelector('#edit-project-modal input[name="ExistingProfileImagePath"]').value = image;

            const preview = document.querySelector('#edit-project-preview');
            const containerImage = document.querySelector('[data-container="upload-edit-project"]');
            const icon = document.querySelector('[data-icon="upload-edit-project"]');

            if (image) {
                preview.src = '/' + image.replace(/^\/+/, '');
                preview.classList.remove('hide');
                containerImage.classList.add('selected');
                icon.classList.replace('fa-camera', 'fa-pen');
            }

            const quillEditor = document.querySelector('#edit-project-description-wysiwyg-editor')?._quill;
            if (quillEditor) {
                quillEditor.root.innerHTML = description;
            }

            const members = button.dataset.members;
            let preSelectedUserTags = [];

            try {
                const parsedMembers = JSON.parse(members);
                preSelectedUserTags = parsedMembers.map(user => {
                    return {
                        id: user.id,
                        fullName: `${user.firstName} ${user.lastName}`,
                        imageUrl: user.profileImage ?? 'default.svg'
                    };
                });

            } catch (e) {
                console.error('Kunde inte parsa members:', members, e);
            }

            // Tog hjälp av ChatGPT för att rensa taggar från andra projekt.
            const container = document.getElementById('tagged-users-edit');
            if (container) {
                container.querySelectorAll('.user-tag').forEach(tag => tag.remove()); // Tar bort tidigare taggar
                const input = container.querySelector('.form-tag-input');
                if (input) input.value = ''; // Rensar sökfältsvärde.

                const results = document.getElementById('user-search-results-edit');
                if (results) results.innerHTML = ''; // Tömmer sökresultat
            }
            preSelectedUserTags.forEach(user => {
                console.log("User ID:", user.id);  // Kontrollera att id är definierat
            });
            initTagSelector({
                containerId: 'tagged-users-edit',
                inputId: 'user-search-edit',
                resultsId: 'user-search-results-edit',
                searchUrl: (query) => '/Projects/SearchUsers?term=' + encodeURIComponent(query),
                displayProperty: 'fullName',
                imageProperty: 'imageUrl',
                tagClass: 'user-tag',
                emptyMessage: 'No users found',
                preselected: preSelectedUserTags
            });


        });
    });

    document.querySelectorAll('[data-action="edit-client"]').forEach(button => {
        button.addEventListener('click', () => {
            const id = button.dataset.memberId;
            const name = button.dataset.name;
            const email = button.dataset.email;
            const phone = button.dataset.phone;


            document.querySelector('#edit-client-modal input[name="Id"]').value = id;
            document.querySelector('#edit-client-modal input[name="ClientName"]').value = name;
            document.querySelector('#edit-client-modal input[name="Email"]').value = email;
            document.querySelector('#edit-client-modal input[name="PhoneNumber"]').value = phone;
        });
    });

    document.querySelectorAll('[data-action="edit-member"]').forEach(button => {
        button.addEventListener('click', () => {
            const id = button.dataset.memberId;
            const firstname = button.dataset.firstname;
            const lastname = button.dataset.lastname;
            const email = button.dataset.email;
            const phone = button.dataset.phone;
            const jobtitle = button.dataset.jobtitle;
            const address = button.dataset.address;
            const profileimage = button.dataset.profileimage; 

            document.querySelector('#edit-member-modal input[name="Id"]').value = id;
            document.querySelector('#edit-member-modal input[name="FirstName"]').value = firstname;
            document.querySelector('#edit-member-modal input[name="LastName"]').value = lastname;
            document.querySelector('#edit-member-modal input[name="Email"]').value = email;
            document.querySelector('#edit-member-modal input[name="PhoneNumber"]').value = phone;
            document.querySelector('#edit-member-modal input[name="JobTitle"]').value = jobtitle;
            document.querySelector('#edit-member-modal input[name="Address.Address"]').value = address;
            document.querySelector('#edit-member-modal input[name="ExistingProfileImagePath"]').value = profileimage;

            const preview = document.querySelector('#edit-profile-preview');
            const container = document.querySelector('[data-container="upload-edit"]');
            const icon = document.querySelector('[data-icon="upload-edit"]');

            if (profileimage) {
                preview.src = '/' + profileimage.replace(/^\/+/, ''); //Tog hjälp utav ChatGPT här, det blev fel path, så var tvungen att replacea \ samt lägga till /.
                preview.classList.remove('hide');
                container.classList.add('selected');
                icon.classList.replace('fa-camera', 'fa-pen');
            }

        });
    });


    // Form-select dropdowns
    document.querySelectorAll('.form-select').forEach(select => {
        const trigger = select.querySelector('.form-select-trigger')
        const triggerText = trigger.querySelector('.form-select-text')
        const options = select.querySelectorAll('.form-select-option')
        const hiddenInput = select.querySelector('input[type="hidden"]')
        const placeholder = select.dataset.placeholder || "Choose"

        const setValue = (value = "", text = placeholder) => {
            triggerText.textContent = text
            hiddenInput.value = value
            select.classList.toggle('has-placeholder', !value)
        }

        setValue()

        trigger.addEventListener('click', (e) => {
            e.stopPropagation()
            document.querySelectorAll('.form-select.open')
                .forEach(el => el !== select && el.classList.remove('open'))
            select.classList.toggle('open')
        })

        options.forEach(option =>
            option.addEventListener('click', () => {
                setValue(option.dataset.value, option.textContent)
                select.classList.remove('open')
            })
        )

        document.addEventListener('click', e => {
            if (!select.contains(e.target)) select.classList.remove('open')
        })
    })


    // Populate Dates - Tog hjälp utav ChatGPT för att populera dagar/månader/år
    document.querySelectorAll('.form-group-date').forEach(dateOfBirth => {

        const daySelect = dateOfBirth.querySelector('[data-type="day"]');
        const monthSelect = dateOfBirth.querySelector('[data-type="month"]');
        const yearSelect = dateOfBirth.querySelector('[data-type="year"]');

        const triggerSelect = (li, selectEl) => {
            const triggerText = selectEl.querySelector('.form-select-text');
            const hiddenInput = selectEl.querySelector('input[type="hidden"]');
            const placeholder = selectEl.dataset.placeholder || "Choose";

            triggerText.textContent = li.textContent;
            hiddenInput.value = li.dataset.value;
            selectEl.classList.toggle('has-placeholder', !li.dataset.value);
            selectEl.classList.remove('open');
        };


        for (let i = 1; i <= 31; i++) {
            const li = document.createElement('li'); //Skapar nytt li element.
            li.classList.add('form-select-option'); //Lägger på vår CSS klass.
            li.dataset.value = i; //Lägger på nuvarande värde på i
            li.textContent = i; //Sätter textinnehållet i li till i-värdet.
            li.addEventListener('click', () => triggerSelect(li, daySelect.parentElement)); //Anropar triggerSelect när li klickas.
            daySelect?.appendChild(li); //lägger till li elementet i listan
        }

        const months = [
            'January', 'February', 'March', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December'
        ];

        months.forEach((month, index) => {
            const li = document.createElement('li');
            li.classList.add('form-select-option');
            li.dataset.value = index + 1;
            li.textContent = month;
            li.addEventListener('click', () => triggerSelect(li, monthSelect.parentElement));
            monthSelect?.appendChild(li);
        });

        const currentYear = new Date().getFullYear();
        for (let y = currentYear; y >= 1900; y--) {
            const li = document.createElement('li');
            li.classList.add('form-select-option');
            li.dataset.value = y;
            li.textContent = y;
            li.addEventListener('click', () => triggerSelect(li, yearSelect.parentElement));
            yearSelect?.appendChild(li);
        }
    });


    //handle submit forms
    const forms = document.querySelectorAll('form')
    forms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault()

            clearErrorMessages(form)
            const formData = new FormData(form)
            try {
                const res = await fetch(form.action, {
                    method: 'post',
                    body: formData
                })
                if (res.ok) {
                    const modal = form.closest('.modal')
                    if (modal)
                        modal.style.display = 'none';

                    window.location.reload()
                }
                else if (res.status === 400) {
                    const data = await res.json() 
                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
                            let input = form.querySelector(`[name="${key}"]`)
                            if (input) {
                                input.classList.add('input-validation-error')
                            }

                            let span = form.querySelector(`[data-valmsg-for="${key}"]`)
                            if (span) {
                                span.innerText = data.errors[key].join('\n');
                                span.classList.add('field-validation-error')
                            }
                        })
                    }
                    if (data.message) {
                        const globalError = form.querySelector('[data-error-global]');
                        if (globalError) {
                            globalError.innerText = data.message;
                            globalError.classList.add('open');
                        } else {
                            alert(data.message); 
                        }
                    }
                }
                
            }
            catch {
                console.log('error submitting the form.')
            }
        })
    })

    function clearErrorMessages(form) {
        form.querySelectorAll('[data-val="true"]').forEach(input => {
            input.classList.remove('input-validation-error')
        })

        form.querySelectorAll('[data-valmsg-for]').forEach(span => {
            span.innerText = ''
            span.classList.remove('field-validation-error')
        })
    }


    //DARKMODE
    //Tog hjälp av ChatGPT för att få det att fungera ihop med cookies.
    //Valde att köra så att DarkMode utan godkända funktionella cookies ska kunna köras, och körs på hela sidan tills de stänger ner sidan
    //När de stänger ner sidan / öppnar sidan i en ny flik, så ska dark mode inte vara aktiverat.
    const darkModeToggle = document.getElementById('darkModeToggle');

    
    if (!darkModeToggle) return;

    //Här läses det från cookies, om det finns sparat. Men det går att använda darkmode oavsett funktionellt samtycke, men sparas inte.
    const consentCookie = getCookie("cookieConsent");
    if (consentCookie) {
        const consent = JSON.parse(consentCookie);

        //Om funktionella cookies är godkända, så kollas cookien för darkMode
        if (consent.functional) {
            const savedTheme = getCookie("darkMode");
            if (savedTheme === "enabled") {
                document.body.classList.add('dark-mode');
                darkModeToggle.checked = true;
            }
        } else {
            const savedTheme = sessionStorage.getItem("darkMode");
            if (savedTheme === "enabled") {
                document.body.classList.add('dark-mode');
                darkModeToggle.checked = true;
            }
        }
    }

    darkModeToggle.addEventListener('change', function () {
       
        const consentCookie = getCookie("cookieConsent");
        if (consentCookie) {
            const consent = JSON.parse(consentCookie);
            if (consent.functional) {
                
                if (this.checked) {
                    document.body.classList.add('dark-mode');
                    setCookie("darkMode", "enabled", 30); //Sparar dark mode om funktionella cookies är godkända, 30 dagar.
                } else {
                    document.body.classList.remove('dark-mode');
                    setCookie("darkMode", "disabled", 30); // 
                }
            } else {
                if (this.checked) {
                    document.body.classList.add('dark-mode');
                    sessionStorage.setItem("darkMode", "enabled"); //Här sparas det till sessionStorage
                } else {
                    document.body.classList.remove('dark-mode');
                    sessionStorage.setItem("darkMode", "disabled");  //Tas bort från sessionStorage.
                }
            }
        }
    });

})

function updateRelativeTimes() {
    const elements = document.querySelectorAll('.notification .time');
    const now = new Date();

    elements.forEach(el => {
        const created = new Date(el.getAttribute('data-created'));
        const diff = now - created;
        const diffSeconds = Math.floor(diff / 1000);
        const diffMinutes = Math.floor(diffSeconds / 60);
        const diffHours = Math.floor(diffMinutes / 60);
        const diffDays = Math.floor(diffHours / 24);
        const diffWeeks = Math.floor(diffDays / 7);

        let relativeTime = '';

        if (diffMinutes < 1) {
            relativeTime = '0 min ago';
        } else if (diffMinutes < 60) {
            relativeTime = diffMinutes + ' min ago';
        } else if (diffHours < 2) {
            relativeTime = diffHours + ' hour ago';
        } else if (diffHours < 24) {
            relativeTime = diffHours + ' hours ago';
        } else if (diffDays < 2) {
            relativeTime = diffDays + ' day ago';
        } else if (diffDays < 7) {
            relativeTime = diffDays + ' days ago';
        } else {
            relativeTime = diffWeeks + ' weeks ago'
        }
        el.textContent = relativeTime;
    });
}


